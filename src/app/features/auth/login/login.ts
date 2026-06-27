import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth';
import { LoginCredentials } from '../../../core/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  credentials: LoginCredentials = {
    identifier: '',
    password: ''
  };

  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  returnUrl: string | null = null;

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
  }

  onSubmit(): void {
    if (!this.credentials.identifier || !this.credentials.password) {
      this.errorMessage.set('Please enter your email/username and password.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        
        if (response && response.id) {
          if (response.role === 'SystemUser' && response.employeeProfile) {
            this.router.navigate(['/admin/dashboard']);
          } 
          else if (response.role === 'Provider') {
            this.router.navigate(['/provider/dashboard']);
          } 
          else {
            const destination = this.returnUrl || '/';
            this.router.navigateByUrl(destination);
          }
        } else {
          this.errorMessage.set('Login processing failed. Please try again.');
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err?.error?.message || 'Invalid credentials. Please try again.');
      }
    });
  }
}