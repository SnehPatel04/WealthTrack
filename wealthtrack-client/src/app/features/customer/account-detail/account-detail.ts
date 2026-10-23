import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { AccountService, Account } from '../../../core/services/account.service';
import { PortfolioService, PortfolioSummary } from '../../../core/services/portfolio.service';
import { TransactionService, Transaction } from '../../../core/services/transaction.service';

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

@Component({
  selector: 'app-account-detail',
  imports: [CurrencyPipe, DatePipe, DecimalPipe, RouterLink],
  templateUrl: './account-detail.html',
  styleUrl: './account-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountDetail implements OnInit {
  private readonly route              = inject(ActivatedRoute);
  private readonly accountService     = inject(AccountService);
  private readonly portfolioService   = inject(PortfolioService);
  private readonly transactionService = inject(TransactionService);

  protected readonly status       = signal<FetchStatus>('idle');
  protected readonly errorMsg     = signal('');
  protected readonly account      = signal<Account | null>(null);
  protected readonly portfolio    = signal<PortfolioSummary | null>(null);
  protected readonly transactions = signal<Transaction[]>([]);

  ngOnInit() {
    const accountId = Number(this.route.snapshot.paramMap.get('accountId'));
    this.loadData(accountId);
  }

  private loadData(accountId: number) {
    this.status.set('loading');

    this.accountService.getAccountById(accountId).subscribe({
      next: (account) => {
        this.account.set(account);
        this.portfolioService.getPortfolioByAccount(accountId).subscribe({
          next: (portfolio) => {
            this.portfolio.set(portfolio);
            this.transactionService.getTransactionsForAccount(accountId).subscribe({
              next: (txns) => {
                this.transactions.set(txns);
                this.status.set('success');
              },
              error: () => {
                this.transactions.set([]);
                this.status.set('success');
              },
            });
          },
          error: () => {
            this.errorMsg.set('Failed to load portfolio data.');
            this.status.set('error');
          },
        });
      },
      error: () => {
        this.errorMsg.set('Account not found or you do not have permission to view it.');
        this.status.set('error');
      },
    });
  }
}
