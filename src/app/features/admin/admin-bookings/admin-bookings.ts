import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth/auth';
import { Booking, BookingStatus } from '../../../core/models/booking.model';

@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-bookings.html',
  styleUrls: ['./admin-bookings.css']
})
export class AdminBookings implements OnInit {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7012/api/Booking';

  isAdmin = this.authService.isAdmin; 

  bookings = signal<Booking[]>([]);
  isLoading = signal<boolean>(true);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const endpoint = this.isAdmin ? this.apiUrl : `${this.apiUrl}/user`;

    this.http.get<Booking[]>(endpoint).subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to pull the master global reservations ledger.');
        this.isLoading.set(false);
      }
    });
  }

  updateStatus(id: number, numericStatus: string): void {
    this.isSaving.set(true);
    this.errorMessage.set(null);

    // PUT /api/Booking/{id}/status matching request body schema { status: 0 }
    this.http.put(`${this.apiUrl}/${id}/status`, { status: Number(numericStatus) }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Reservation lifecycle flag modified successfully.');
        this.loadBookings();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to update transaction status.');
      }
    });
  }

  cancelBooking(id: number): void {
    if (!confirm('Are you sure you want to terminate this reservation?')) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);

    // POST /api/Booking/{id}/cancel matching path param definition
    this.http.post(`${this.apiUrl}/${id}/cancel`, {}).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Reservation canceled successfully.');
        this.loadBookings();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to execute cancellation.');
      }
    });
  }

  getStatusNumericValue(status: string): number {
    switch (status) {
      case 'Pending': return BookingStatus.Pending;
      case 'Confirmed': return BookingStatus.Confirmed;
      case 'Canceled': return BookingStatus.Canceled;
      case 'Completed': return BookingStatus.Completed;
      case 'Rejected': return BookingStatus.Rejected;
      default: return 0;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending': return 'bg-amber-100 text-amber-800 border-amber-400';
      case 'Confirmed': return 'bg-emerald-100 text-emerald-800 border-emerald-400';
      case 'Canceled': return 'bg-rose-100 text-rose-800 border-rose-400';
      case 'Completed': return 'bg-blue-100 text-blue-800 border-blue-400';
      case 'Rejected': return 'bg-neutral-100 text-neutral-700 border-neutral-400';
      default: return 'bg-white text-black border-black';
    }
  }
}