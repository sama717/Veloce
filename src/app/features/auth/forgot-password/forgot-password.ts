import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router'; // 👈 Removed Router since we don't force redirect anymore
import { AuthService } from '../../../core/services/auth/auth';
import { ResetPasswordData } from '../../../core/models/auth.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './forgot-password.html'
})
export class ForgotPassword {
  private authService = inject(AuthService);

  email = '';
  codeSent = signal<boolean>(false);
  isResetSuccess = signal<boolean>(false); 
  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  resetPayload: ResetPasswordData = {
    token: '',
    newPassword: '',
    confirmPassword: ''
  };

  onRequestCode(): void {
    if (!this.email) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.forgotPassword({ email: this.email }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.codeSent.set(true);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to send verification code.');
      }
    });
  }

  onResetPassword(): void {
    if (this.resetPayload.newPassword !== this.resetPayload.confirmPassword) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.resetPassword(this.resetPayload).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.isResetSuccess.set(true); 
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err?.error?.message || 'Invalid code or code has expired.');
      }
    });
  }
}