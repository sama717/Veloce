import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './navbar.html',
})
export class Navbar {
  public authService = inject(AuthService); // ✅ Made public
  private router = inject(Router);

  isMenuOpen = false;

  isAuthenticated = this.authService.isAuthenticated;

  getUsername(): string {
    const user = this.authService.currentUser();
    if (!user) return 'User';
    return `${user.firstName || ''} ${user.lastName || ''}`.trim() || 'User';
  }

  getInitials(): string {
    const user = this.authService.currentUser();
    if (!user) return '?';
    return `${(user.firstName?.[0] || '')}${(user.lastName?.[0] || '')}`.toUpperCase() || '?';
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  onLogout(): void {
    this.isMenuOpen = false;
    this.authService.logout();
    this.router.navigate(['/']);
  }
}