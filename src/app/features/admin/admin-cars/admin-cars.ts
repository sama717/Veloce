import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CarService } from '../../../core/services/car';
import { AuthService } from '../../../core/services/auth/auth';
import { Car } from '../../../core/models/car.model';

@Component({
  selector: 'app-admin-cars',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-cars.html',
  styleUrls: ['./admin-cars.css']
})
export class AdminCars implements OnInit {
  private carService = inject(CarService);
  private authService = inject(AuthService);

  isAdmin = this.authService.isAdmin;
  isManager = this.authService.isManager;

  cars = signal<Car[]>([]);
  filteredCars = signal<Car[]>([]);
  isLoading = signal<boolean>(true);
  isDeleting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Filters
  statusFilter = signal<string>('all');

  ngOnInit(): void {
    this.loadCars();
  }

  loadCars(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.carService.getCars({}).subscribe({
      next: (data) => {
        this.cars.set(data);
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load cars.');
        this.isLoading.set(false);
      }
    });
  }

  applyFilters(): void {
    const status = this.statusFilter();
    let filtered = this.cars();

    if (status !== 'all') {
      filtered = filtered.filter(car => car.status === status);
    }

    this.filteredCars.set(filtered);
  }

  deleteCar(id: number): void {
    if (!confirm('Are you sure you want to delete this car?')) return;

    this.isDeleting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.carService.deleteCar(id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.successMessage.set('Car deleted successfully!');
        this.loadCars();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to delete car.');
        setTimeout(() => this.errorMessage.set(null), 3000);
      }
    });
  }

  getStatusBadge(status: string): string {
    const map: Record<string, string> = {
      'Available': 'bg-green-100 text-green-800 border-green-300',
      'Rented': 'bg-yellow-100 text-yellow-800 border-yellow-300',
      'Sold': 'bg-blue-100 text-blue-800 border-blue-300',
      'Deleted': 'bg-gray-100 text-gray-800 border-gray-300'
    };
    return map[status] || 'bg-gray-100 text-gray-800';
  }

  getTypeLabel(type: string): string {
    return type || 'Unknown';
  }

  getConditionLabel(condition: string): string {
    return condition || 'Unknown';
  }

  getMainImage(car: Car): string {
    if (!car.imageUrls || car.imageUrls.length === 0) {
      return 'https://placehold.co/600x400/e5e7eb/6b7280?text=No+Image';
    }
    return car.imageUrls[0];
  }
}