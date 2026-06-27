import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CarService } from '../../../core/services/car/car';
import { AuthService } from '../../../core/services/auth/auth';
import { Car, CarFilterParams } from '../../../core/models/car.model';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
})
export class Catalog implements OnInit {
  private carService = inject(CarService);
  private authService = inject(AuthService);
  private router = inject(Router);

  isAuthenticated = this.authService.isAuthenticated;

  cars = signal<Car[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  filters = signal<CarFilterParams>({
    brand: '',
    model: '',
    color: '',
    condition: undefined,
    yearFrom: undefined,
    yearTo: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    type: undefined,
  });

  ngOnInit(): void {
    this.loadCatalog();
  }

  loadCatalog(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.carService.getCars(this.filters()).subscribe({
      next: (data) => {
        this.cars.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set('Failed to load vehicles.');
        console.error(err);
      },
    });
  }

  applyFilters(): void {
    this.loadCatalog();
  }

  clearFilters(): void {
    this.filters.set({
      brand: '',
      model: '',
      color: '',
      condition: undefined,
      yearFrom: undefined,
      yearTo: undefined,
      minPrice: undefined,
      maxPrice: undefined,
      type: undefined,
    });
    this.loadCatalog();
  }

  getMainImage(car: Car): string {
    if (!car.imageUrls || car.imageUrls.length === 0) {
      return 'https://placehold.co/600x400/e5e7eb/6b7280?text=No+Image';
    }
    return car.imageUrls[0];
  }

  viewDetails(carId: number): void {
    this.router.navigate(['/cars', carId]);
  }

  updateFilter(key: keyof CarFilterParams, value: any): void {
    let parsedValue: any = value;

    if (value === '' || value === 'undefined' || value === null || value === undefined) {
      parsedValue = undefined;
    } else if (key === 'minPrice' || key === 'maxPrice' || key === 'type' || key === 'condition') {
      parsedValue = Number(value);
      if (isNaN(parsedValue)) parsedValue = undefined;
    } else if (typeof value === 'string') {
      parsedValue = value.trim();
      if (parsedValue === '') parsedValue = undefined;
    }

    this.filters.update((prev) => ({
      ...prev,
      [key]: parsedValue,
    }));

    this.loadCatalog();
  }
}