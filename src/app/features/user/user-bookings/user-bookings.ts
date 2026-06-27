import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BookingService } from '../../../core/services/booking/booking';
import { CarService } from '../../../core/services/car';
import { DealershipService } from '../../../core/services/dealership';
import { AuthService } from '../../../core/services/auth/auth';
import { Booking } from '../../../core/models/booking.model';

@Component({
  selector: 'app-user-bookings',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './user-bookings.html',
  styleUrls: ['./user-bookings.css']
})
export class UserBookings implements OnInit {
  private bookingService = inject(BookingService);
  private carService = inject(CarService);
  private dealershipService = inject(DealershipService);
  private authService = inject(AuthService);

  bookings = signal<Booking[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  isProvider = this.authService.isProvider;
  isSystemUser = this.authService.isSystemUser;
  isCustomer = this.authService.isCustomer;

  // ✅ Status mapping for dropdown
  statusMap: Record<string, number> = {
    'Pending': 0,
    'Confirmed': 1,
    'Canceled': 2,
    'Completed': 3,
    'Rejected': 4
  };

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading.set(true);
    
    if (this.isProvider) {
      this.bookingService.getProviderBookings().subscribe({
        next: (data) => {
          this.enrichBookings(data);
        },
        error: () => {
          this.errorMessage.set('Failed to load bookings for your cars.');
          this.isLoading.set(false);
        }
      });
    } else if (this.isSystemUser) {
      this.bookingService.getUserBookings().subscribe({
        next: (data) => {
          this.enrichBookings(data);
        },
        error: () => {
          this.errorMessage.set('Failed to load all bookings.');
          this.isLoading.set(false);
        }
      });
    } else {
      this.bookingService.getUserBookings().subscribe({
        next: (data) => {
          this.enrichBookings(data);
        },
        error: () => {
          this.errorMessage.set('Failed to load your bookings.');
          this.isLoading.set(false);
        }
      });
    }
  }

  private enrichBookings(data: Booking[]): void {
    const requests = data.map(async (booking) => {
      if (booking.carId) {
        try {
          const car = await this.carService.getCarById(booking.carId).toPromise();
          booking.car = car;
        } catch {}
      }
      if (booking.consultationDetail?.dealershipId) {
        try {
          const dealer = await this.dealershipService.getDealership(booking.consultationDetail.dealershipId).toPromise();
          booking.consultationDetail.dealership = dealer;
        } catch {}
      }
      return booking;
    });

    Promise.all(requests).then((enrichedBookings) => {
      this.bookings.set(enrichedBookings);
      this.isLoading.set(false);
    });
  }

  downloadContract(bookingId: number): void {
    this.bookingService.getRentalContract(bookingId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `RentalContract_${bookingId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage.set('Failed to download contract.');
      }
    });
  }

  cancelBooking(bookingId: number): void {
    if (!confirm('Are you sure you want to cancel this booking?')) return;
    this.bookingService.cancelBooking(bookingId).subscribe({
      next: () => {
        this.loadBookings();
      },
      error: () => {
        this.errorMessage.set('Failed to cancel booking.');
      }
    });
  }

  // ✅ Convert string status to number
  updateStatus(bookingId: number, status: string): void {
    const statusNumber = this.statusMap[status];
    if (statusNumber === undefined) {
      this.errorMessage.set('Invalid status value.');
      return;
    }
    this.bookingService.updateBookingStatus(bookingId, { status: statusNumber }).subscribe({
      next: () => {
        this.loadBookings();
      },
      error: () => {
        this.errorMessage.set('Failed to update booking status.');
      }
    });
  }

  getStatusBadge(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bg-yellow-100 text-yellow-800 border-yellow-300',
      'Confirmed': 'bg-green-100 text-green-800 border-green-300',
      'Canceled': 'bg-red-100 text-red-800 border-red-300',
      'Completed': 'bg-blue-100 text-blue-800 border-blue-300',
      'Rejected': 'bg-gray-100 text-gray-800 border-gray-300'
    };
    return map[status] || 'bg-gray-100 text-gray-800';
  }
}