import { ChangeDetectionStrategy, Component, OnInit, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { AccountService, Account } from '../../../core/services/account.service';
import { PortfolioService, PortfolioSummary, Holding } from '../../../core/services/portfolio.service';
import { TransactionService, Transaction } from '../../../core/services/transaction.service';

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

interface AssetGroup {
  assetClass: string;
  marketValue: number;
  percent: number;
}

@Component({
  selector: 'app-client-detail',
  imports: [CurrencyPipe, DatePipe, DecimalPipe, RouterLink],
  templateUrl: './client-detail.html',
  styleUrl: './client-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientDetail implements OnInit {
  private readonly route              = inject(ActivatedRoute);
  private readonly accountService     = inject(AccountService);
  private readonly portfolioService   = inject(PortfolioService);
  private readonly transactionService = inject(TransactionService);

  protected readonly clientId   = signal<number>(0);
  protected readonly status     = signal<FetchStatus>('idle');
  protected readonly errorMsg   = signal('');
  protected readonly accounts   = signal<Account[]>([]);
  protected readonly portfolios = signal<PortfolioSummary[]>([]);
  protected readonly transactions = signal<Transaction[][]>([]);

  protected readonly clientName = computed(() => {
    const accts = this.accounts();
    if (accts.length === 0) return 'Client';
    return 'Client Profile';
  });

  protected readonly totalPortfolioValue = computed(() =>
    this.portfolios().reduce((sum, p) => sum + p.totalMarketValue, 0)
  );

  protected readonly totalGainLoss = computed(() =>
    this.portfolios().reduce((sum, p) => sum + p.totalGainLossAmount, 0)
  );

  protected readonly allHoldings = computed<Holding[]>(() =>
    this.portfolios().flatMap(p => p.holdings)
  );

  protected readonly allTransactions = computed<Transaction[]>(() =>
    this.transactions().flat().sort((a, b) =>
      new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime()
    )
  );

  protected readonly assetGroups = computed<AssetGroup[]>(() => {
    const holdings = this.allHoldings();
    if (holdings.length === 0) return [];
    const totalValue = holdings.reduce((sum, h) => sum + h.marketValue, 0);
    if (totalValue === 0) return [];
    const groups = new Map<string, number>();
    for (const h of holdings) {
      groups.set(h.assetClass, (groups.get(h.assetClass) ?? 0) + h.marketValue);
    }
    return Array.from(groups.entries())
      .map(([assetClass, marketValue]) => ({
        assetClass,
        marketValue,
        percent: Math.round((marketValue / totalValue) * 1000) / 10,
      }))
      .sort((a, b) => b.marketValue - a.marketValue);
  });

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('clientId'));
    this.clientId.set(id);
    this.loadClientData(id);
  }

  private loadClientData(clientId: number) {
    this.status.set('loading');

    this.accountService.getAccountsForClient(clientId).subscribe({
      next: (accounts) => {
        this.accounts.set(accounts);
        this.portfolioService.getPortfoliosForClient(clientId).subscribe({
          next: (portfolios) => {
            this.portfolios.set(portfolios);
            const txnRequests = accounts.map(a =>
              new Promise<Transaction[]>(resolve => {
                this.transactionService.getTransactionsForAccount(a.id).subscribe({
                  next: (txns) => resolve(txns),
                  error: () => resolve([]),
                });
              })
            );
            Promise.all(txnRequests).then(allTxns => {
              this.transactions.set(allTxns);
              this.status.set('success');
            });
          },
          error: () => {
            this.errorMsg.set('Failed to load portfolio data.');
            this.status.set('error');
          },
        });
      },
      error: () => {
        this.errorMsg.set('Client not found or you do not have access.');
        this.status.set('error');
      },
    });
  }
}
