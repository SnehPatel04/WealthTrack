import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AccountService, Account } from '../../../core/services/account.service';

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

@Component({
  selector: 'app-customer-accounts',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './customer-accounts.html',
  styleUrl: './customer-accounts.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerAccounts implements OnInit {
  private readonly accountService = inject(AccountService);

  protected readonly status   = signal<FetchStatus>('idle');
  protected readonly accounts = signal<Account[]>([]);
  protected readonly errorMsg = signal('');

  ngOnInit() {
    this.status.set('loading');
    this.accountService.getMyAccounts().subscribe({
      next: (data) => {
        this.accounts.set(data);
        this.status.set('success');
      },
      error: () => {
        this.errorMsg.set('Failed to load your accounts.');
        this.status.set('error');
      },
    });
  }
}
