import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CarService } from '../../../core/services/car';
import { AuthService } from '../../../core/services/auth/auth';
import { DealershipService } from '../../../core/services/dealership';
import { Dealership } from '../../../core/models/dealership.model';

@Component({
  selector: 'app-list-car',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './list-car.html',
  styleUrls: ['./list-car.css']
})
export class ListCar implements OnInit {
  private fb = inject(FormBuilder);
  private carService = inject(CarService);
  private authService = inject(AuthService);
  private dealershipService = inject(DealershipService);
  public router = inject(Router);

  carForm!: FormGroup;
  isSubmitting = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  selectedFiles: File[] = [];
  previewUrls: string[] = [];
  dealerships: Dealership[] = [];
  isLoadingDealerships = signal<boolean>(false);
  isProvider = signal<boolean>(true);

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
    this.initForm(user.id, isProvider);

    if (isSystemUser) {
      this.loadDealerships();
    }
  }

  private initForm(ownerId: number, isProvider: boolean): void {
    this.carForm = this.fb.group({
      brand: ['', [Validators.required]],
      model: ['', [Validators.required]],
      year: [new Date().getFullYear(), [Validators.required, Validators.min(1900)]],
      color: ['', [Validators.required]],
      mileage: [0, [Validators.required, Validators.min(0)]],
      seats: [5, [Validators.required, Validators.min(1)]],
      description: [''],
      type: ['Rent', [Validators.required]],        // ✅ String
      condition: ['New', [Validators.required]],    // ✅ String
      price: [null],
      pricePerDay: [null],
      quantity: [1, [Validators.required, Validators.min(1)]],
      dealershipId: [isProvider ? null : 0],
      ownerId: [isProvider ? ownerId : null]
    });

    this.carForm.get('type')?.valueChanges.subscribe((type) => {
      this.carForm.get('price')?.clearValidators();
      this.carForm.get('pricePerDay')?.clearValidators();

      if (type === 'Sale') {
        this.carForm.get('price')?.setValidators([Validators.required, Validators.min(0)]);
      } else {
        this.carForm.get('pricePerDay')?.setValidators([Validators.required, Validators.min(0)]);
      }
      this.carForm.get('price')?.updateValueAndValidity();
      this.carForm.get('pricePerDay')?.updateValueAndValidity();
    });
  }

  get listingType(): string {
    return this.carForm.get('type')?.value ?? 'Rent';
  }

  loadDealerships(): void {
    this.isLoadingDealerships.set(true);
    this.dealershipService.getDealerships().subscribe({
      next: (data) => {
        this.dealerships = data;
        this.isLoadingDealerships.set(false);
      },
      error: () => {
        this.isLoadingDealerships.set(false);
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

  onSubmit(): void {
    if (this.carForm.invalid) {
      this.errorMessage.set('Please fill out all required fields.');
      this.carForm.markAllAsTouched();
      return;
    }

    if (!this.isProvider() && !this.carForm.get('dealershipId')?.value) {
      this.errorMessage.set('Please select a dealership.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const formData = new FormData();
    const raw = this.carForm.value;

    // Convert strings to numbers for backend
    const typeNumber = raw.type === 'Sale' ? 0 : 1;
    const conditionNumber = raw.condition === 'New' ? 0 : 1;

    // Append all fields, but override type and condition with numbers
    Object.entries(raw).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        if (key === 'type') {
          formData.append(key, typeNumber.toString());
        } else if (key === 'condition') {
          formData.append(key, conditionNumber.toString());
        } else {
          formData.append(key, value.toString());
        }
      }
    });

    this.selectedFiles.forEach(file => {
      formData.append('Images', file);
    });

    this.carService.createCar(formData).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.successMessage.set('Car listed successfully!');
        setTimeout(() => {
          this.router.navigate(['/my-cars']);
        }, 2000);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to list car.');
      }
    });
  }
}