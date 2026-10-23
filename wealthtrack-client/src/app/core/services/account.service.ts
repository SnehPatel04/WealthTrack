import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Account {
  id: number;
  clientId: number;
  accountNumber: string;
  accountType: string;
  balance: number;
  availableCash: number;
  status: string;
  openedAt: string;
}

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/accounts`;

  getMyAccounts(): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.base}/my`);
  }

  getAccountsForClient(clientId: number): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.base}/client/${clientId}`);
  }

  getAccountById(id: number): Observable<Account> {
    return this.http.get<Account>(`${this.base}/${id}`);
  }
}
