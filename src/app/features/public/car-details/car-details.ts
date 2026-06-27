import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CarService } from '../../../core/services/car/car';
import { AuthService } from '../../../core/services/auth/auth'; 
import { Car } from '../../../core/models/car.model';

@Component({
  selector: 'app-car-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './car-details.html',
  styleUrl: './car-details.css'
})
export class CarDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private carService = inject(CarService);
  private authService = inject(AuthService); 

  isAuthenticated = this.authService.isAuthenticated;
  
  car = signal<Car | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  activeImageIndex = signal<number>(0);

  ngOnInit(): void {
    const carIdStr = this.route.snapshot.paramMap.get('id');
    if (!carIdStr) {
      this.errorMessage.set('Invalid vehicle resource references.');
      this.isLoading.set(false);
      return;
    }

    const carId = parseInt(carIdStr, 10);
    this.loadVehicleDetails(carId);
  }

  loadVehicleDetails(id: number): void {
    this.carService.getCarById(id).subscribe({
      next: (data: Car) => {
        this.car.set(data);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set('Failed to pull specifications for this asset unit.');
        this.isLoading.set(false);
        console.error(err);
      }
    });
  }

  handleAction(car: Car): void {
    if (!this.isAuthenticated()) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: `/booking/create/${car.id}` } 
      });
      return;
    }
    
    if (car.type === 'Rent') {
      this.router.navigate(['/booking/create', car.id]); 
    } else {
      this.router.navigate(['/booking/create', car.id]); 
    }
  }

  selectHeroImage(index: number): void {
    this.activeImageIndex.set(index);
  }

  navigateBackToCatalog(): void {
    this.router.navigate(['/catalog']);
  }
}