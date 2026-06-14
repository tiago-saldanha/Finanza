import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Rotas públicas (sem layout)
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent),
  },

  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
  },

  // Rotas protegidas (com layout)
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'transactions',
        loadComponent: () =>
          import('./features/transactions/transaction-list/transaction-list.component').then(
            m => m.TransactionListComponent
          ),
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/categories/category-list/category-list.component').then(
            m => m.CategoryListComponent
          ),
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./features/auth/change-password/change-password.component').then(
            m => m.ChangePasswordComponent
          ),
      },
      {
        path: 'cash-flow',
        loadComponent: () =>
          import('./features/cash-flow/cash-flow.component').then(m => m.CashFlowComponent),
      },
      {
        path: 'assets-liabilities',
        loadComponent: () =>
          import('./features/assets-liabilities/assets-liabilities.component').then(m => m.AssetsLiabilitiesComponent),
      },
      {
        path: 'patrimony',
        loadComponent: () =>
          import('./features/patrimony/patrimony.component').then(m => m.PatrimonyComponent),
      },
      {
        path: 'planning',
        loadComponent: () =>
          import('./features/planning/planning.component').then(m => m.PlanningComponent),
      },
      {
        path: 'financial-accounts',
        loadComponent: () =>
          import('./features/financial-accounts/financial-account-list/financial-account-list.component').then(
            m => m.FinancialAccountListComponent
          ),
      },
      {
        path: 'fire',
        loadComponent: () =>
          import('./features/fire/fire.component').then(m => m.FireComponent),
      },
      {
        path: 'goals',
        loadComponent: () =>
          import('./features/goals/goals.component').then(m => m.GoalsComponent),
      },
      {
        path: 'investments',
        loadComponent: () =>
          import('./features/investments/investments.component').then(m => m.InvestmentsComponent),
      },
      {
        path: 'patrimony-snapshots',
        loadComponent: () =>
          import('./features/patrimony-snapshots/patrimony-snapshots.component').then(
            m => m.PatrimonySnapshotsComponent
          ),
      },
      {
        path: 'loans',
        loadComponent: () =>
          import('./features/loans/loans.component').then(m => m.LoansComponent),
      },
      {
        path: 'loan-payables',
        loadComponent: () =>
          import('./features/loan-payables/loan-payables.component').then(m => m.LoanPayablesComponent),
      },
      {
        path: 'account',
        loadComponent: () =>
          import('./features/account/account.component').then(m => m.AccountComponent),
      },
      { path: '**', redirectTo: 'dashboard' },
    ],
  },
];
