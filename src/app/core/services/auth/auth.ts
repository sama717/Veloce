import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, throwError } from 'rxjs';
import {
  LoginCredentials,
  AuthResponse,
  RegisterData,
  ResetPasswordData,
  ForgotPasswordData,
} from '../../models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api/Auth';

  private readonly httpOptions = {};

  currentUser = signal<AuthResponse | null>(null);
  isAuthenticated = signal<boolean>(false);

  constructor() {
    this.restoreSession();
  }

  login(credentials: LoginCredentials): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials, this.httpOptions).pipe(
      tap((response: AuthResponse) => {
        if (response) {
          if (response.token) {
            localStorage.setItem('veloce_jwt', response.token); // ✅ Store JWT
          }
          if (response.refreshToken) {
            localStorage.setItem('veloce_rt', response.refreshToken);
          }
          this.currentUser.set(response);
          this.isAuthenticated.set(true);
        }
      }),
    );
  }

  register(data: RegisterData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, data, this.httpOptions);
  }

  forgotPassword(data: ForgotPasswordData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/forgot-password`, data, this.httpOptions);
  }

  resetPassword(data: ResetPasswordData): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/reset-password`, data, this.httpOptions);
  }

  logout(): void {
    localStorage.removeItem('veloce_jwt');
    localStorage.removeItem('veloce_rt');
    this.http.post(`${this.apiUrl}/logout`, {}, this.httpOptions).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession(),
    });
  }

  refreshAuth(): Observable<AuthResponse> {
    const token = localStorage.getItem('veloce_rt');
    if (!token) {
      this.logout();
      return throwError(() => new Error('No refresh token'));
    }

    return this.http
      .post<AuthResponse>(`${this.apiUrl}/refresh`, { refreshToken: token }, this.httpOptions)
      .pipe(
        tap((response: AuthResponse) => {
          if (response) {
            if (response.token) {
              localStorage.setItem('veloce_jwt', response.token); // ✅ Update JWT
            }
            if (response.refreshToken) {
              localStorage.setItem('veloce_rt', response.refreshToken);
            }
            this.currentUser.set(response);
            this.isAuthenticated.set(true);
          }
        }),
        catchError((error) => {
          this.logout();
          return throwError(() => error);
        }),
      );
  }

  private restoreSession(): void {
    const token = localStorage.getItem('veloce_jwt');
    if (!token) {
      this.clearSession();
      return;
    }
    this.http.get<AuthResponse>(`${this.apiUrl}/profile`, this.httpOptions).subscribe({
      next: (user: AuthResponse) => {
        this.currentUser.set(user);
        this.isAuthenticated.set(true);
        if (user.refreshToken) {
          localStorage.setItem('veloce_rt', user.refreshToken);
        }
      },
      error: () => {
        this.clearSession();
      },
    });
  }

  private clearSession(): void {
    localStorage.removeItem('veloce_jwt');
    localStorage.removeItem('veloce_rt');
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  changePassword(currentPassword: string, newPassword: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/change-password`,
      { currentPassword, newPassword },
      this.httpOptions,
    );
  }

  changeEmail(newEmail: string, password: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/change-email`, { newEmail, password }, this.httpOptions);
  }

  verifyEmailChange(token: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-email-change`, { token }, this.httpOptions);
  }

  changePhone(newPhoneNumber: string, password: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/change-phone`,
      { newPhoneNumber, password },
      this.httpOptions,
    );
  }

  verifyPhoneChange(token: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-phone-change`, { token }, this.httpOptions);
  }

  resendVerificationEmail(): Observable<any> {
    return this.http.post(`${this.apiUrl}/resend-verification-email`, {}, this.httpOptions);
  }

  updateCurrentUser(updatedUser: Partial<AuthResponse>): void {
    const current = this.currentUser();
    if (current) {
      this.currentUser.set({
        ...current,
        ...updatedUser,
      });
    }
  }

  get isCustomer(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.clientProfile?.userMode === 'Customer';
  }

  get isProvider(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.clientProfile?.userMode === 'Provider';
  }

  get isSystemUser(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.role === 'SystemUser';
  }

  get isAdmin(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.role === 'SystemUser' && user.employeeProfile?.position === 'Admin';
  }

  get isManager(): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.role === 'SystemUser' && user.employeeProfile?.position === 'Manager';
  }

  get userRole(): string {
    const user = this.currentUser();
    if (!user) return 'guest';
    if (user.role === 'SystemUser') {
      return user.employeeProfile?.position === 'Admin' ? 'admin' : 'manager';
    }
    if (user.clientProfile?.userMode === 'Provider') return 'provider';
    if (user.clientProfile?.userMode === 'Customer') return 'customer';
    return 'guest';
  }

  verifyEmail(code: string): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/verify-email`,
      JSON.stringify(code), 
      { headers: { 'Content-Type': 'application/json' } },
    );
  }
}
