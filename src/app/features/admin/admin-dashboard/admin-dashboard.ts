import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth';
import { AdminService } from '../../../core/services/admin';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin-dashboard.css']
})
export class AdminDashboard implements OnInit {
  private authService = inject(AuthService);
  private adminService = inject(AdminService);

  isAdmin = this.authService.isAdmin;
  isManager = this.authService.isManager;

  stats = signal({
    totalUsers: 0,
    totalCars: 0,
    totalBookings: 0,
    totalRevenue: 0,
    pendingBookings: 0
  });
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.adminService.getStats().subscribe({
      next: (data) => {
        this.stats.set({
          totalUsers: data.totalUsers || 0,
          totalCars: data.totalCars || 0,
          totalBookings: data.totalBookings || 0,
          totalRevenue: data.totalRevenue || 0,
          pendingBookings: data.pendingBookings || 0
        });
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set('Failed to load dashboard stats.');
        console.error('Stats error:', err);
      }
    });
  }
}