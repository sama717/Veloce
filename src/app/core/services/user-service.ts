import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserProfile, UpdateUserProfileDto } from '../../core/models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api/User';

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/profile`);
  }

  updateProfile(data: UpdateUserProfileDto): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/profile`, data);
  }

  updateProfilePicture(file: File): Observable<UserProfile> {
    const formData = new FormData();
    formData.append('profilePicture', file);
    return this.http.put<UserProfile>(`${this.apiUrl}/profile/picture`, formData);
  }

  deactivateAccount(password: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/deactivate`, { password });
  }

  deleteAccount(password: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/delete`, { password });
  }

  assignEmployee(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/employees`, data);
  }

  updateEmployee(userId: number, data: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/employees/${userId}`, data);
  }

  removeEmployee(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/employees/${userId}`);
  }

  getEmployeesByDealership(dealershipId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/employees/dealership/${dealershipId}`);
  }

  getAllEmployees(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/employees/all`);
  }

  getAllUsers(): Observable<UserProfile[]> {
    return this.http.get<UserProfile[]>(`${this.apiUrl}/users`);
  }
}
