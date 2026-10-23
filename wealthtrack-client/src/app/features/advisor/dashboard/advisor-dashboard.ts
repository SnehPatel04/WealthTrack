import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ReportService, AdvisorReport } from '../../../core/services/report.service';
import { RequestService, ClientRequest } from '../../../core/services/request.service';

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

@Component({
  selector: 'app-advisor-dashboard',
  imports: [CurrencyPipe, DatePipe, ReactiveFormsModule, RouterLink],
  templateUrl: './advisor-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdvisorDashboard implements OnInit {
  private readonly fb             = inject(FormBuilder);
  private readonly authService    = inject(AuthService);
  private readonly reportService  = inject(ReportService);
  private readonly requestService = inject(RequestService);

  protected readonly firstName      = this.authService.userFirstName;
  protected readonly status         = signal<FetchStatus>('idle');
  protected readonly report         = signal<AdvisorReport | null>(null);
  protected readonly requests       = signal<ClientRequest[]>([]);
  protected readonly errorMessage   = signal('');
  protected readonly selectedRequest = signal<ClientRequest | null>(null);
  protected readonly respondStatus   = signal<'idle' | 'submitting' | 'success' | 'error'>('idle');
  protected readonly respondMessage  = signal('');

  protected readonly respondForm = this.fb.group({
    advisorResponse: ['', Validators.required],
    status:          ['InProgress', Validators.required],
  });

  ngOnInit() {
    this.loadDashboard();
  }

  protected loadDashboard() {
    this.status.set('loading');
    this.reportService.getAdvisorReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.status.set('success');
      },
      error: () => {
        this.errorMessage.set('Failed to load dashboard data.');
        this.status.set('error');
      },
    });
    this.loadRequests();
  }

  protected loadRequests() {
    this.requestService.getAdvisorInbox().subscribe({
      next: (data) => this.requests.set(data),
      error: () => {},
    });
  }

  protected selectRequest(req: ClientRequest) {
    this.selectedRequest.set(req);
    this.respondStatus.set('idle');
    this.respondMessage.set('');
    this.respondForm.patchValue({
      advisorResponse: req.advisorResponse || '',
      status: req.status === 'Pending' ? 'InProgress' : req.status,
    });
  }

  protected clearSelected() {
    this.selectedRequest.set(null);
    this.respondForm.reset({ status: 'InProgress' });
  }

  protected submitResponse() {
    if (this.respondForm.invalid) {
      return;
    }

    const req = this.selectedRequest();
    if (!req) {
      return;
    }

    this.respondStatus.set('submitting');

    const value = this.respondForm.value;

    this.requestService.respondToRequest(req.id, {
      advisorResponse: value.advisorResponse!,
      status:          value.status!,
    }).subscribe({
      next: () => {
        this.respondStatus.set('success');
        this.respondMessage.set('Response saved.');
        this.selectedRequest.set(null);
        this.respondForm.reset({ status: 'InProgress' });
        this.loadRequests();
        this.loadDashboard();
      },
      error: () => {
        this.respondStatus.set('error');
        this.respondMessage.set('Failed to save response. Please try again.');
      },
    });
  }
}
