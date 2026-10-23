import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  advisorProfileId: number | null;
  clientProfileId: number | null;
}

export interface AssignAdvisor {
  advisorId: number;
  clientId: number;
}

export interface CreateUser {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/users`;

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.base);
  }

  setActiveStatus(userId: number, active: boolean): Observable<void> {
    const action = active ? 'activate' : 'deactivate';
    return this.http.patch<void>(`${this.base}/${userId}/${action}`, {});
  }

  assignAdvisor(dto: AssignAdvisor): Observable<void> {
    return this.http.post<void>(`${this.base}/assign-advisor`, dto);
  }

  createUser(dto: CreateUser): Observable<User> {
    return this.http.post<User>(this.base, dto);
  }
}
