import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Transaction {
  id: number;
  investmentAccountId: number;
  securityId: number | null;
  ticker: string;
  securityName: string;
  transactionType: string;
  quantity: number;
  pricePerShare: number;
  totalAmount: number;
  notes: string;
  transactionDate: string;
}

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/transactions`;

  getTransactionsForAccount(accountId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.base}/account/${accountId}`);
  }
}
