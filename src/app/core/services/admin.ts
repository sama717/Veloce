import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking } from '../../core/models/booking.model';
import { Car } from '../../core/models/car.model';
import { Dealership } from '../../core/models/dealership.model';
import { UserProfile } from '../../core/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api';

  getStats(): Observable<{
    totalUsers: number;
    totalCars: number;
    totalBookings: number;
    totalRevenue: number;
    pendingBookings: number;
  }> {
    return this.http.get<any>(`${this.apiUrl}/Admin/stats`);
  }

  getAllBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/Booking`);
  }

  deleteBooking(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Booking/${id}`);
  }

  getDealerships(): Observable<Dealership[]> {
    return this.http.get<Dealership[]>(`${this.apiUrl}/Dealership`);
  }

  getDealership(id: number): Observable<Dealership> {
    return this.http.get<Dealership>(`${this.apiUrl}/Dealership/${id}`);
  }

  createDealership(data: any): Observable<Dealership> {
    return this.http.post<Dealership>(`${this.apiUrl}/Dealership`, data);
  }

  updateDealership(id: number, data: any): Observable<Dealership> {
    return this.http.put<Dealership>(`${this.apiUrl}/Dealership/${id}`, data);
  }

  deleteDealership(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Dealership/${id}`);
  }

  assignEmployee(data: {
    userId: number;
    dealershipId: number;
    position: number;
  }): Observable<any> {
    return this.http.post(`${this.apiUrl}/User/employees`, data);
  }

  updateEmployee(userId: number, data: {
    dealershipId?: number;
    position?: number;
  }): Observable<any> {
    return this.http.put(`${this.apiUrl}/User/employees/${userId}`, data);
  }

  removeEmployee(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/User/employees/${userId}`);
  }

  getEmployeesByDealership(dealershipId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/User/employees/dealership/${dealershipId}`);
  }

  getAllEmployees(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/User/employees/all`);
  }

  getAllUsers(): Observable<UserProfile[]> {
    return this.http.get<UserProfile[]>(`${this.apiUrl}/User`);
  }

  getUserById(id: number): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/User/${id}`);
  }

  getAllCars(): Observable<Car[]> {
    return this.http.get<Car[]>(`${this.apiUrl}/Car`);
  }

  isAdmin(): boolean {
    const user = JSON.parse(localStorage.getItem('veloce_user') || '{}');
    return user.role === 'SystemUser' && user.employeeProfile?.position === 'Admin';
  }

  isManager(): boolean {
    const user = JSON.parse(localStorage.getItem('veloce_user') || '{}');
    return user.role === 'SystemUser' && user.employeeProfile?.position === 'Manager';
  }
}