import { ChangeDetectionStrategy, Component, OnInit, inject, signal, computed } from '@angular/core';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { RouterLink } from '@angular/router';
import { ChartData, ChartOptions } from 'chart.js';
import { Chart, ArcElement, Tooltip, Legend, DoughnutController } from 'chart.js';
import { AuthService } from '../../../core/services/auth.service';
import { ReportService, CustomerReport } from '../../../core/services/report.service';
import { PortfolioService, PortfolioSummary, Holding } from '../../../core/services/portfolio.service';
import { RequestService, ClientRequest } from '../../../core/services/request.service';

Chart.register(ArcElement, Tooltip, Legend, DoughnutController);

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

interface AssetGroup {
  assetClass: string;
  marketValue: number;
  percent: number;
}

@Component({
  selector: 'app-customer-dashboard',
  imports: [CurrencyPipe, DecimalPipe, DatePipe, ReactiveFormsModule, BaseChartDirective, RouterLink],
  templateUrl: './customer-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CustomerDashboard implements OnInit {
  private readonly fb             = inject(FormBuilder);
  private readonly authService    = inject(AuthService);
  private readonly reportService  = inject(ReportService);
  private readonly portfolioService = inject(PortfolioService);
  private readonly requestService = inject(RequestService);

  protected readonly firstName    = this.authService.userFirstName;
  protected readonly reportStatus = signal<FetchStatus>('idle');
  protected readonly report       = signal<CustomerReport | null>(null);
  protected readonly portfolios   = signal<PortfolioSummary[]>([]);
  protected readonly requests     = signal<ClientRequest[]>([]);
  protected readonly errorMessage = signal('');

  protected readonly submitStatus  = signal<'idle' | 'submitting' | 'success' | 'error'>('idle');
  protected readonly submitMessage = signal('');
  protected readonly showForm      = signal(false);

  protected readonly pendingCount = computed(() =>
    this.requests().filter(r => r.status === 'Pending').length
  );

  protected readonly assetGroups = computed<AssetGroup[]>(() => {
    const allHoldings: Holding[] = this.portfolios().flatMap(p => p.holdings);
    if (allHoldings.length === 0) return [];

    const totalValue = allHoldings.reduce((sum, h) => sum + h.marketValue, 0);
    if (totalValue === 0) return [];

    const groups = new Map<string, number>();
    for (const h of allHoldings) {
      groups.set(h.assetClass, (groups.get(h.assetClass) ?? 0) + h.marketValue);
    }

    return Array.from(groups.entries()).map(([assetClass, marketValue]) => ({
      assetClass,
      marketValue,
      percent: (marketValue / totalValue) * 100,
    })).sort((a, b) => b.marketValue - a.marketValue);
  });

  private readonly CHART_COLORS = ['#237a65', '#4a7adf', '#d4a017', '#e05c5c', '#7c5cbf'];

  protected readonly chartData = computed<ChartData<'doughnut'>>(() => {
    const groups = this.assetGroups();
    return {
      labels: groups.map(g => g.assetClass),
      datasets: [{
        data: groups.map(g => Math.round(g.percent * 100) / 100),
        backgroundColor: groups.map((_, i) => this.CHART_COLORS[i % this.CHART_COLORS.length]),
        borderWidth: 2,
        borderColor: '#ffffff',
      }],
    };
  });

  protected readonly chartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    cutout: '65%',
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (ctx) => ' ' + ctx.label + ': ' + ctx.parsed.toFixed(1) + '%',
        },
      },
    },
  };

  protected readonly requestForm = this.fb.group({
    requestType: ['', Validators.required],
    subject:     ['', Validators.required],
    message:     ['', Validators.required],
  });

  ngOnInit() {
    this.loadDashboard();
  }

  protected loadDashboard() {
    this.reportStatus.set('loading');
    this.reportService.getCustomerReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.reportStatus.set('success');
      },
      error: () => {
        this.errorMessage.set('Failed to load dashboard data.');
        this.reportStatus.set('error');
      },
    });
    this.portfolioService.getMyPortfolios().subscribe({
      next: (data) => this.portfolios.set(data),
      error: () => {},
    });
    this.loadRequests();
  }

  protected loadRequests() {
    this.requestService.getMyRequests().subscribe({
      next: (data) => this.requests.set(data),
      error: () => {},
    });
  }

  protected toggleForm() {
    this.showForm.update(v => !v);
    this.submitStatus.set('idle');
    this.submitMessage.set('');
    this.requestForm.reset();
  }

  protected submitRequest() {
    if (this.requestForm.invalid) return;
    this.submitStatus.set('submitting');
    const value = this.requestForm.value;
    this.requestService.submitRequest({
      requestType: value.requestType!,
      subject: value.subject!,
      message: value.message!,
    }).subscribe({
      next: () => {
        this.submitStatus.set('success');
        this.submitMessage.set('Request submitted successfully.');
        this.requestForm.reset();
        this.showForm.set(false);
        this.loadRequests();
      },
      error: () => {
        this.submitStatus.set('error');
        this.submitMessage.set('Failed to submit request. Please try again.');
      },
    });
  }

  protected getColor(index: number): string {
    return this.CHART_COLORS[index % this.CHART_COLORS.length];
  }
}
