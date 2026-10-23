import { ChangeDetectionStrategy, Component, OnInit, inject, signal, computed } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { ReportService, AdminReport } from '../../../core/services/report.service';
import { UserService, User, CreateUser } from '../../../core/services/user.service';

type FetchStatus = 'idle' | 'loading' | 'success' | 'error';

@Component({
  selector: 'app-admin-dashboard',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './admin-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboard implements OnInit {
  private readonly fb            = inject(FormBuilder);
  private readonly authService   = inject(AuthService);
  private readonly reportService = inject(ReportService);
  private readonly userService   = inject(UserService);

  protected readonly firstName    = this.authService.userFirstName;
  protected readonly status       = signal<FetchStatus>('idle');
  protected readonly report       = signal<AdminReport | null>(null);
  protected readonly errorMessage = signal('');
  protected readonly users        = signal<User[]>([]);

  protected readonly assignStatus  = signal<'idle' | 'submitting' | 'success' | 'error'>('idle');
  protected readonly assignMessage = signal('');

  protected readonly createStatus  = signal<'idle' | 'submitting' | 'success' | 'error'>('idle');
  protected readonly createMessage = signal('');

  protected readonly advisors = computed(() =>
    this.users().filter(u => u.role === 'Advisor' && u.isActive)
  );

  protected readonly customers = computed(() =>
    this.users().filter(u => u.role === 'Customer' && u.isActive)
  );

  protected readonly assignForm = this.fb.group({
    advisorId: ['', Validators.required],
    clientId:  ['', Validators.required],
  });

  protected readonly createForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName:  ['', Validators.required],
    email:     ['', [Validators.required, Validators.email]],
    password:  ['', [Validators.required, Validators.minLength(8)]],
    role:      ['', Validators.required],
  });

  ngOnInit() {
    this.loadDashboard();
  }

  protected loadDashboard() {
    this.status.set('loading');
    this.reportService.getAdminReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.status.set('success');
      },
      error: () => {
        this.errorMessage.set('Failed to load dashboard data.');
        this.status.set('error');
      },
    });
    this.loadUsers();
  }

  private loadUsers() {
    this.userService.getAllUsers().subscribe({
      next: (data) => this.users.set(data),
      error: () => {},
    });
  }

  protected toggleUserStatus(user: User) {
    this.userService.setActiveStatus(user.id, !user.isActive).subscribe({
      next: () => {
        this.users.update(list =>
          list.map(u => u.id === user.id ? { ...u, isActive: !u.isActive } : u)
        );
      },
      error: () => {},
    });
  }

  protected assignAdvisor() {
    if (this.assignForm.invalid) {
      return;
    }

    this.assignStatus.set('submitting');

    const value = this.assignForm.value;
    const selectedAdvisor = this.advisors().find(a => a.id === Number(value.advisorId));
    const selectedClient  = this.customers().find(c => c.id === Number(value.clientId));

    if (!selectedAdvisor?.advisorProfileId || !selectedClient?.clientProfileId) {
      this.assignStatus.set('error');
      this.assignMessage.set('Could not find advisor or client profile. Please try again.');
      return;
    }

    this.userService.assignAdvisor({
      advisorId: selectedAdvisor.advisorProfileId,
      clientId:  selectedClient.clientProfileId,
    }).subscribe({
      next: () => {
        this.assignStatus.set('success');
        this.assignMessage.set('Advisor assigned successfully.');
        this.assignForm.reset();
      },
      error: () => {
        this.assignStatus.set('error');
        this.assignMessage.set('Assignment failed. Please check selections and try again.');
      },
    });
  }

  protected createUser() {
    if (this.createForm.invalid) {
      return;
    }

    this.createStatus.set('submitting');
    this.createMessage.set('');

    const value = this.createForm.value;

    const dto: CreateUser = {
      firstName: value.firstName!,
      lastName:  value.lastName!,
      email:     value.email!,
      password:  value.password!,
      role:      value.role!,
    };

    this.userService.createUser(dto).subscribe({
      next: (newUser) => {
        this.createStatus.set('success');
        this.createMessage.set(newUser.firstName + ' ' + newUser.lastName + ' was created successfully.');
        this.createForm.reset();
        this.loadUsers();
        this.reportService.getAdminReport().subscribe({
          next: (data) => this.report.set(data),
          error: () => {},
        });
      },
      error: (err) => {
        this.createStatus.set('error');
        if (err.status === 400) {
          this.createMessage.set('Invalid input. Check all fields and try again.');
        } else if (err.status === 409) {
          this.createMessage.set('A user with this email address already exists.');
        } else {
          this.createMessage.set('Failed to create user. Please try again.');
        }
      },
    });
  }
}
