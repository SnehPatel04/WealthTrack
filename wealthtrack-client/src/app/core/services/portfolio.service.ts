import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Holding {
  id: number;
  investmentAccountId: number;
  securityId: number;
  ticker: string;
  securityName: string;
  assetClass: string;
  quantity: number;
  averageCostBasis: number;
  currentPrice: number;
  marketValue: number;
  gainLossAmount: number;
  gainLossPercent: number;
  allocationPercent: number;
}

export interface PortfolioSummary {
  accountId: number;
  accountNumber: string;
  accountType: string;
  totalMarketValue: number;
  availableCash: number;
  totalGainLossAmount: number;
  totalGainLossPercent: number;
  holdings: Holding[];
}

@Injectable({ providedIn: 'root' })
export class PortfolioService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/portfolios`;

  getMyPortfolios(): Observable<PortfolioSummary[]> {
    return this.http.get<PortfolioSummary[]>(`${this.base}/my`);
  }

  getPortfolioByAccount(accountId: number): Observable<PortfolioSummary> {
    return this.http.get<PortfolioSummary>(`${this.base}/account/${accountId}`);
  }

  getPortfoliosForClient(clientId: number): Observable<PortfolioSummary[]> {
    return this.http.get<PortfolioSummary[]>(`${this.base}/client/${clientId}`);
  }
}
