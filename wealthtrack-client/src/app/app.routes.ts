import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/customer/login/login').then((m) => m.Login),
  },
  {
    path: 'customer',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Customer' },
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/customer/dashboard/customer-dashboard').then(
            (m) => m.CustomerDashboard,
          ),
      },
      {
        path: 'accounts',
        loadComponent: () =>
          import('./features/customer/accounts/customer-accounts').then(
            (m) => m.CustomerAccounts,
          ),
      },
      {
        path: 'accounts/:accountId',
        loadComponent: () =>
          import('./features/customer/account-detail/account-detail').then(
            (m) => m.AccountDetail,
          ),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  {
    path: 'advisor',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Advisor' },
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/advisor/dashboard/advisor-dashboard').then(
            (m) => m.AdvisorDashboard,
          ),
      },
      {
        path: 'clients/:clientId',
        loadComponent: () =>
          import('./features/advisor/client-detail/client-detail').then(
            (m) => m.ClientDetail,
          ),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/admin/dashboard/admin-dashboard').then(
            (m) => m.AdminDashboard,
          ),
      },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./shared/components/unauthorized/unauthorized').then(
        (m) => m.Unauthorized,
      ),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./shared/components/not-found/not-found').then(
        (m) => m.NotFound,
      ),
  },
];


