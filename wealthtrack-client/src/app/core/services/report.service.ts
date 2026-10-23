import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AccountSummary {
  accountId: number;
  accountNumber: string;
  accountType: string;
  balance: number;
  availableCash: number;
}

export interface CustomerReport {
  totalPortfolioValue: number;
  totalGainLoss: number;
  totalGainLossPercent: number;
  totalAccounts: number;
  accounts: AccountSummary[];
}

export interface ClientSummary {
  clientId: number;
  clientName: string;
  totalPortfolioValue: number;
  accountCount: number;
}

export interface AdvisorReport {
  totalManagedAssets: number;
  totalClients: number;
  pendingRequests: number;
  clients: ClientSummary[];
}

export interface AdminReport {
  totalUsers: number;
  totalCustomers: number;
  totalAdvisors: number;
  activeUsers: number;
  inactiveUsers: number;
  totalAccounts: number;
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/reports`;

  getCustomerReport(): Observable<CustomerReport> {
    return this.http.get<CustomerReport>(`${this.base}/customer`);
  }

  getAdvisorReport(): Observable<AdvisorReport> {
    return this.http.get<AdvisorReport>(`${this.base}/advisor`);
  }

  getAdminReport(): Observable<AdminReport> {
    return this.http.get<AdminReport>(`${this.base}/admin`);
  }
}
