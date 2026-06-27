import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user-service';
import { AuthService } from '../../../core/services/auth/auth';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-settings.html',
  styleUrls: ['./user-settings.css']
})
export class UserSettings {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);

  // Deactivation
  deactivatePassword = '';
  isDeactivating = signal<boolean>(false);
  deactivateError = signal<string | null>(null);
  deactivateSuccess = signal<string | null>(null);

  // Deletion
  deletePassword = '';
  isDeleting = signal<boolean>(false);
  deleteError = signal<string | null>(null);
  deleteSuccess = signal<string | null>(null);

  confirmDelete = signal<boolean>(false);

  deactivateAccount(): void {
    if (!this.deactivatePassword) {
      this.deactivateError.set('Please enter your password.');
      return;
    }

    this.isDeactivating.set(true);
    this.deactivateError.set(null);
    this.deactivateSuccess.set(null);

    this.userService.deactivateAccount(this.deactivatePassword).subscribe({
      next: () => {
        this.isDeactivating.set(false);
        this.deactivateSuccess.set('Account deactivated successfully.');
        // Optionally log out after a delay
        setTimeout(() => {
          this.authService.logout();
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err) => {
        this.isDeactivating.set(false);
        this.deactivateError.set(err?.error?.message || 'Failed to deactivate account. Check your password.');
      }
    });
  }

  deleteAccount(): void {
    if (!this.deletePassword) {
      this.deleteError.set('Please enter your password.');
      return;
    }

    if (!this.confirmDelete()) {
      this.deleteError.set('Please confirm that you want to permanently delete your account.');
      return;
    }

    this.isDeleting.set(true);
    this.deleteError.set(null);
    this.deleteSuccess.set(null);

    this.userService.deleteAccount(this.deletePassword).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.deleteSuccess.set('Account permanently deleted.');
        setTimeout(() => {
          this.authService.logout();
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.deleteError.set(err?.error?.message || 'Failed to delete account. Check your password.');
      }
    });
  }

  toggleConfirmDelete(): void {
    this.confirmDelete.set(!this.confirmDelete());
  }
}