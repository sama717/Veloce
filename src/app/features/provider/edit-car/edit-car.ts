import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CarService } from '../../../core/services/car';
import { AuthService } from '../../../core/services/auth/auth';
import { DealershipService } from '../../../core/services/dealership';
import { Dealership } from '../../../core/models/dealership.model';
import { Car } from '../../../core/models/car.model';

@Component({
  selector: 'app-edit-car',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-car.html',
  styleUrls: ['./edit-car.css']
})
export class EditCar implements OnInit {
  private fb = inject(FormBuilder);
  private carService = inject(CarService);
  private authService = inject(AuthService);
  private dealershipService = inject(DealershipService);
  private route = inject(ActivatedRoute);
  public router = inject(Router);

  carId = signal<number>(0);
  isProvider = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  existingImages: any[] = [];
  selectedFiles: File[] = [];
  previewUrls: string[] = [];
  imagesToDelete: number[] = [];
  dealerships: Dealership[] = [];

  carForm: FormGroup = this.fb.group({
    brand: ['', [Validators.required]],
    model: ['', [Validators.required]],
    year: ['', [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
    color: ['', [Validators.required]],
    mileage: [0, [Validators.required, Validators.min(0)]],
    seats: [5, [Validators.required, Validators.min(1), Validators.max(20)]],
    description: [''],
    price: [null],
    pricePerDay: [null],
    quantity: [1, [Validators.required, Validators.min(1)]],
    type: ['Rent', [Validators.required]], 
    condition: ['New', [Validators.required]], 
    status: [0],
    dealershipId: [0],
    ownerId: [null],
  });

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (!user) {
      this.router.navigate(['/login']);
      return;
    }

    const isProvider = user.clientProfile?.userMode === 'Provider';
    const isSystemUser = user.role === 'SystemUser';
    if (!isProvider && !isSystemUser) {
      this.router.navigate(['/']);
      return;
    }

    this.isProvider.set(isProvider);

    if (isSystemUser) {
      this.loadDealerships();
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/my-cars']);
      return;
    }
    this.carId.set(parseInt(id));
    this.loadCar(this.carId());

    this.carForm.get('type')?.valueChanges.subscribe((type) => {
      this.updatePriceValidators(type);
    });
  }

  private updatePriceValidators(type: string): void {
    this.carForm.get('price')?.clearValidators();
    this.carForm.get('pricePerDay')?.clearValidators();

    if (type === 'Sale') {
      this.carForm.get('price')?.setValidators([Validators.required, Validators.min(0)]);
    } else {
      this.carForm.get('pricePerDay')?.setValidators([Validators.required, Validators.min(0)]);
    }
    this.carForm.get('price')?.updateValueAndValidity();
    this.carForm.get('pricePerDay')?.updateValueAndValidity();
  }

  loadCar(id: number): void {
    this.isLoading.set(true);
    this.carService.getCarById(id).subscribe({
      next: (data: any) => {
        this.carForm.patchValue({
          brand: data.brand,
          model: data.model,
          year: data.year,
          color: data.color,
          mileage: data.mileage,
          seats: data.seats,
          description: data.description || '',
          price: data.price || null,
          pricePerDay: data.pricePerDay || null,
          quantity: data.quantity,
          type: data.type, 
          condition: data.condition, 
          status: data.status,
          dealershipId: data.dealershipId || 0,
          ownerId: data.ownerId || null,
        });
        this.existingImages = data.images || [];
        this.updatePriceValidators(data.type);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load car details.');
        this.isLoading.set(false);
      }
    });
  }

  get listingType(): string {
    return this.carForm.get('type')?.value ?? 'Rent';
  }

  loadDealerships(): void {
    this.dealershipService.getDealerships().subscribe({
      next: (data) => {
        this.dealerships = data;
      },
      error: () => {
        console.error('Failed to load dealerships');
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files);
      this.selectedFiles.push(...files);

      files.forEach(file => {
        const reader = new FileReader();
        reader.onload = () => {
          this.previewUrls.push(reader.result as string);
        };
        reader.readAsDataURL(file);
      });
    }
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
    this.previewUrls.splice(index, 1);
  }

  removeExistingImage(imageId: number): void {
    if (this.existingImages.length <= 1) {
      this.errorMessage.set('Cannot delete the last image.');
      return;
    }
    this.existingImages = this.existingImages.filter(img => img.id !== imageId);
    this.imagesToDelete.push(imageId);
  }

  onSubmit(): void {
    if (this.carForm.invalid) {
      this.errorMessage.set('Please fill out all required fields correctly.');
      this.carForm.markAllAsTouched();
      return;
    }

    const formData = new FormData();
    const values = this.carForm.value;

    Object.entries(values).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        formData.append(key, value.toString());
      }
    });

    this.imagesToDelete.forEach(id => {
      formData.append('ImageIdsToDelete', id.toString());
    });

    this.selectedFiles.forEach(file => {
      formData.append('NewImages', file);
    });

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.carService.updateCar(this.carId(), formData).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.successMessage.set('Car updated successfully!');
        setTimeout(() => {
          this.router.navigate(['/my-cars']);
        }, 2000);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to update car.');
      }
    });
  }

  get f() {
    return this.carForm.controls;
  }
}