import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CarService } from '../../../core/services/car';
import { AuthService } from '../../../core/services/auth/auth';
import { Car } from '../../../core/models/car.model';

@Component({
  selector: 'app-my-cars',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-cars.html',
  styleUrls: ['./my-cars.css'],
})
export class MyCars implements OnInit {
  private carService = inject(CarService);
  private authService = inject(AuthService);

  cars = signal<Car[]>([]);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCars();
  }

  loadCars(): void {
    const user = this.authService.currentUser();
    if (!user) {
      this.errorMessage.set('Please log in first.');
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);
    this.carService.getMyCars().subscribe({
      next: (data) => {
        this.cars.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load your cars.');
        this.isLoading.set(false);
      },
    });
  }

  deleteCar(id: number): void {
    if (!confirm('Are you sure you want to delete this car?')) return;
    this.carService.deleteCar(id).subscribe({
      next: () => {
        this.cars.update((cars) => cars.filter((car) => car.id !== id));
      },
      error: () => {
        this.errorMessage.set('Failed to delete car.');
      },
    });
  }

  getTypeLabel(type: string): string {
    return type;
  }

  getConditionLabel(condition: string): string {
    return condition;
  }
}