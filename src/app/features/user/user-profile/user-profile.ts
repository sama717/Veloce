import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user-service';
import { AuthService } from '../../../core/services/auth/auth';
import { UserProfile, UpdateUserProfileDto } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-profile.html',
  styleUrls: ['./user-profile.css'],
})
export class UserProfileComponent implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);

  profile = signal<UserProfile | null>(null);
  isLoading = signal<boolean>(true);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Profile update
  updateData: UpdateUserProfileDto = { firstName: '', middleName: '', lastName: '' };

  // Password change
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';

  // Email change
  newEmail = '';
  emailPassword = '';
  emailChangeToken = '';
  showEmailVerification = false;

  // Phone change
  newPhoneNumber = '';
  phonePassword = '';
  phoneChangeToken = '';
  showPhoneVerification = false;

  // Profile picture
  selectedFile: File | null = null;
  previewUrl: string | null = null;

  // ✅ Email verification
  verificationCode = '';
  isSendingVerification = signal<boolean>(false);
  isVerifying = signal<boolean>(false);
  verificationMessage = signal<string | null>(null);

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.isLoading.set(true);
    this.userService.getProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.authService.updateCurrentUser({
          firstName: data.firstName,
          middleName: data.middleName || '',
          lastName: data.lastName,
          profilePicture: data.profilePicture || null,
          email: data.email,
          username: data.username,
          role: data.role,
          isEmailVerified: data.isEmailVerified,
          clientProfile: data.clientProfile,
          employeeProfile: data.employeeProfile,
        });
        this.updateData = {
          firstName: data.firstName,
          middleName: data.middleName || '',
          lastName: data.lastName,
        };
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load profile');
        this.isLoading.set(false);
      },
    });
  }

  updateProfile() {
    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.userService.updateProfile(this.updateData).subscribe({
      next: (data) => {
        this.profile.set(data);
        this.authService.updateCurrentUser({
          firstName: data.firstName,
          middleName: data.middleName || '',
          lastName: data.lastName,
          profilePicture: data.profilePicture || null,
        });
        this.isSaving.set(false);
        this.successMessage.set('Profile updated!');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to update profile');
      },
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      const reader = new FileReader();
      reader.onload = () => (this.previewUrl = reader.result as string);
      reader.readAsDataURL(this.selectedFile);
    }
  }

  uploadProfilePicture() {
    if (!this.selectedFile) {
      this.errorMessage.set('Please select a file');
      return;
    }
    this.isSaving.set(true);
    this.userService.updateProfilePicture(this.selectedFile).subscribe({
      next: (data) => {
        this.profile.set(data);
        this.authService.updateCurrentUser({
          profilePicture: data.profilePicture || null,
        });
        this.isSaving.set(false);
        this.successMessage.set('Profile picture updated!');
        this.selectedFile = null;
        this.previewUrl = null;
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to update picture');
      },
    });
  }

  changePassword() {
    if (!this.currentPassword || !this.newPassword || !this.confirmPassword) {
      this.errorMessage.set('Please fill in all password fields');
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage.set('Passwords do not match');
      return;
    }
    if (this.newPassword.length < 6) {
      this.errorMessage.set('Password must be at least 6 characters');
      return;
    }
    this.isSaving.set(true);
    this.authService.changePassword(this.currentPassword, this.newPassword).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Password changed!');
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to change password');
      },
    });
  }

  requestEmailChange() {
    if (!this.newEmail || !this.emailPassword) {
      this.errorMessage.set('Please enter new email and password');
      return;
    }
    this.isSaving.set(true);
    this.authService.changeEmail(this.newEmail, this.emailPassword).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showEmailVerification = true;
        this.successMessage.set('Verification code sent to new email');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to request email change');
      },
    });
  }

  verifyEmailChange() {
    if (!this.emailChangeToken) {
      this.errorMessage.set('Please enter the verification code');
      return;
    }
    this.isSaving.set(true);
    this.authService.verifyEmailChange(this.emailChangeToken).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Email changed successfully!');
        this.showEmailVerification = false;
        this.newEmail = '';
        this.emailPassword = '';
        this.emailChangeToken = '';
        this.loadProfile();
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Invalid verification code');
      },
    });
  }

  requestPhoneChange() {
    if (!this.newPhoneNumber || !this.phonePassword) {
      this.errorMessage.set('Please enter new phone and password');
      return;
    }
    this.isSaving.set(true);
    this.authService.changePhone(this.newPhoneNumber, this.phonePassword).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showPhoneVerification = true;
        this.successMessage.set('Verification code sent to email');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Failed to request phone change');
      },
    });
  }

  verifyPhoneChange() {
    if (!this.phoneChangeToken) {
      this.errorMessage.set('Please enter the verification code');
      return;
    }
    this.isSaving.set(true);
    this.authService.verifyPhoneChange(this.phoneChangeToken).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Phone number changed!');
        this.showPhoneVerification = false;
        this.newPhoneNumber = '';
        this.phonePassword = '';
        this.phoneChangeToken = '';
        this.loadProfile();
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'Invalid verification code');
      },
    });
  }

  // ===========================
  // ✅ Email verification methods
  // ===========================

  resendVerification(): void {
    this.isSendingVerification.set(true);
    this.verificationMessage.set(null);

    this.authService.resendVerificationEmail().subscribe({
      next: () => {
        this.isSendingVerification.set(false);
        this.verificationMessage.set('Verification email sent! Please check your inbox.');
      },
      error: (err) => {
        this.isSendingVerification.set(false);
        this.verificationMessage.set(err?.error?.message || 'Failed to send verification email.');
      },
    });
  }

  verifyEmail(): void {
    if (!this.verificationCode) {
      this.verificationMessage.set('Please enter the verification code.');
      return;
    }

    this.isVerifying.set(true);
    this.verificationMessage.set(null);

    this.authService.verifyEmail(this.verificationCode).subscribe({
      next: () => {
        this.isVerifying.set(false);
        this.verificationMessage.set('Email verified successfully!');
        this.verificationCode = '';
        this.loadProfile(); 
      },
      error: (err) => {
        this.isVerifying.set(false);
        this.verificationMessage.set(err?.error?.message || 'Invalid verification code.');
      },
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}