import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  userId: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http   = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly TOKEN_KEY = 'wealthtrack_token';
  private readonly USER_KEY  = 'wealthtrack_user';

  private readonly currentUserSignal = signal<LoginResponse | null>(
    this.loadUserFromStorage()
  );

  readonly currentUser = this.currentUserSignal.asReadonly();

  readonly isLoggedIn = computed(() => this.currentUserSignal() !== null);

  readonly userRole = computed(() => this.currentUserSignal()?.role ?? '');

  readonly userId = computed(() => this.currentUserSignal()?.userId ?? 0);

  readonly userFirstName = computed(() => this.currentUserSignal()?.firstName ?? '');

  login(request: LoginRequest) {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        tap((response) => {
          localStorage.setItem(this.TOKEN_KEY, response.token);
          localStorage.setItem(this.USER_KEY, JSON.stringify(response));
          this.currentUserSignal.set(response);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSignal.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  redirectToDashboard() {
    const role = this.userRole();

    if (role === 'Admin') {
      this.router.navigate(['/admin/dashboard']);
    } else if (role === 'Advisor') {
      this.router.navigate(['/advisor/dashboard']);
    } else {
      this.router.navigate(['/customer/dashboard']);
    }
  }

  private loadUserFromStorage(): LoginResponse | null {
    const stored = localStorage.getItem(this.USER_KEY);
    return stored ? (JSON.parse(stored) as LoginResponse) : null;
  }
}
