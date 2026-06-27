import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../core/services/admin';
import { AuthService } from '../../../core/services/auth/auth';
import { Dealership } from '../../../core/models/dealership.model';

@Component({
  selector: 'app-admin-dealerships',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-dealerships.html',
  styleUrls: ['./admin-dealerships.css']
})
export class AdminDealerships implements OnInit {
  private adminService = inject(AdminService);
  private authService = inject(AuthService);

  isAdmin = this.authService.isAdmin;

  dealerships = signal<Dealership[]>([]);
  isLoading = signal<boolean>(true);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Create form
  showCreateForm = signal<boolean>(false);
  newDealership = {
    name: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    state: '',
    country: ''
  };

  // Edit mode
  editingId = signal<number | null>(null);
  editData = {
    name: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    state: '',
    country: ''
  };

  ngOnInit(): void {
    this.loadDealerships();
  }

  loadDealerships(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.adminService.getDealerships().subscribe({
      next: (data) => {
        this.dealerships.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load dealerships.');
        this.isLoading.set(false);
      }
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm.set(!this.showCreateForm());
    this.newDealership = { name: '', email: '', phoneNumber: '', address: '', city: '', state: '', country: '' };
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  createDealership(): void {
    if (!this.newDealership.name || !this.newDealership.email) {
      this.errorMessage.set('Name and email are required.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.adminService.createDealership(this.newDealership).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Dealership created successfully!');
        this.showCreateForm.set(false);
        this.loadDealerships();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to create dealership.');
        setTimeout(() => this.errorMessage.set(null), 3000);
      }
    });
  }

  startEdit(dealership: Dealership): void {
    this.editingId.set(dealership.id);
    this.editData = {
      name: dealership.name,
      email: dealership.email,
      phoneNumber: dealership.phoneNumber,
      address: dealership.address,
      city: dealership.city,
      state: dealership.state,
      country: dealership.country
    };
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.editData = { name: '', email: '', phoneNumber: '', address: '', city: '', state: '', country: '' };
  }

  saveEdit(id: number): void {
    if (!this.editData.name || !this.editData.email) {
      this.errorMessage.set('Name and email are required.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.adminService.updateDealership(id, this.editData).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Dealership updated successfully!');
        this.editingId.set(null);
        this.loadDealerships();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to update dealership.');
        setTimeout(() => this.errorMessage.set(null), 3000);
      }
    });
  }

  deleteDealership(id: number): void {
    if (!confirm('Are you sure you want to delete this dealership?')) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.adminService.deleteDealership(id).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Dealership deleted successfully!');
        this.loadDealerships();
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to delete dealership.');
        setTimeout(() => this.errorMessage.set(null), 3000);
      }
    });
  }
}