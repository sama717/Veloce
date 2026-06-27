import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserService } from '../../../core/services/user-service';
import { AuthService } from '../../../core/services/auth/auth';
import { UserProfile } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './user-dashboard.html',
  styleUrls: ['./user-dashboard.css']
})
export class UserDashboard implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService);

  profile = signal<UserProfile | null>(null);
  isLoading = signal<boolean>(true);
  isSendingVerification = signal<boolean>(false);
  verificationMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading.set(true);
    this.userService.getProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

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
      }
    });
  }

  getInitials(): string {
    const p = this.profile();
    if (!p) return '?';
    return `${p.firstName?.[0] || ''}${p.lastName?.[0] || ''}`;
  }
}