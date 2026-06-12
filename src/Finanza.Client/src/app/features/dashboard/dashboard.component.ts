import { Component, OnInit, inject, signal, computed, effect } from '@angular/core';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { CommonModule, CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { ThemeService } from '../../core/services/theme.service';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { BaseChartDirective } from 'ng2-charts';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';
import { ChartConfiguration, ChartData } from 'chart.js';
import { Chart, registerables } from 'chart.js';
import { forkJoin } from 'rxjs';

import { TransactionService } from '../../core/services/transaction.service';
import { CategoryService } from '../../core/services/category.service';
import { FinancialAccountService } from '../../core/services/financial-account.service';
import { InvestmentService } from '../../core/services/investment.service';
import { LoanService } from '../../core/services/loan.service';
import { GoalService } from '../../core/services/goal.service';
import { PatrimonyService } from '../../core/services/patrimony.service';
import { PatrimonySnapshotService } from '../../core/services/patrimony-snapshot.service';
import { PlanningService } from '../../core/services/planning.service';

import { Transaction } from '../../core/models/transaction.model';
import { FinancialAccount } from '../../core/models/financial-account.model';
import { InvestmentPortfolio } from '../../core/models/investment.model';
import { LoanSummary } from '../../core/models/loan.model';
import { Goal } from '../../core/models/goal.model';
import { NetWorth } from '../../core/models/patrimony.model';
import { PatrimonySnapshot } from '../../core/models/patrimony-snapshot.model';
import { FinancialPlanning } from '../../core/models/planning.model';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CurrencyPipe,
    DatePipe,
    DecimalPipe,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatChipsModule,
    MatDividerModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatInputModule,
    BaseChartDirective,
    DashboardTabsComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly transactionService   = inject(TransactionService);
  private readonly categoryService      = inject(CategoryService);
  private readonly accountService       = inject(FinancialAccountService);
  private readonly investmentService    = inject(InvestmentService);
  private readonly loanService          = inject(LoanService);
  private readonly goalService          = inject(GoalService);
  private readonly patrimonyService         = inject(PatrimonyService);
  private readonly patrimonySnapshotService = inject(PatrimonySnapshotService);
  private readonly planningService          = inject(PlanningService);
  private readonly themeService             = inject(ThemeService);

  private cssVar(name: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim();
  }

  loading = signal(true);

  transactions      = signal<Transaction[]>([]);
  accounts          = signal<FinancialAccount[]>([]);
  portfolio         = signal<InvestmentPortfolio | null>(null);
  loanSummary       = signal<LoanSummary | null>(null);
  goals             = signal<Goal[]>([]);
  netWorth          = signal<NetWorth | null>(null);
  snapshots         = signal<PatrimonySnapshot[]>([]);
  planning          = signal<FinancialPlanning | null>(null);

  // ---- KPI: Reserva de Emergência ----
  emergencyFundMonths  = computed(() => this.planning()?.emergencyFundMonths ?? 0);
  emergencyFundTarget  = computed(() => this.planning()?.emergencyFundTarget ?? 6);
  emergencyFundPct     = computed(() =>
    Math.min((this.emergencyFundMonths() / this.emergencyFundTarget()) * 100, 100)
  );
  emergencyFundColor   = computed(() => {
    const pct = this.emergencyFundPct();
    if (pct >= 100) return '#2e7d32';
    if (pct >= 50)  return '#f57c00';
    return '#c62828';
  });

  // ---- KPI: Evolução Patrimonial (%) ----
  patrimonialEvolution = computed(() => {
    const snaps = [...this.snapshots()].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
    if (snaps.length < 2) return null;
    const prev = snaps[snaps.length - 2].netWorth;
    const curr = snaps[snaps.length - 1].netWorth;
    if (prev === 0) return null;
    return ((curr - prev) / Math.abs(prev)) * 100;
  });

  // ---- Período (transações do mês) ----
  periodMode = signal<'thisMonth' | 'lastMonth' | 'last30Days' | 'thisYear' | 'all' | 'custom'>('thisMonth');
  showCustomDates = computed(() => this.periodMode() === 'custom');

  customStartCtrl = new FormControl<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
  customEndCtrl   = new FormControl<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0));
  customStart     = signal<Date | null>(this.customStartCtrl.value);
  customEnd       = signal<Date | null>(this.customEndCtrl.value);

  private periodFiltered = computed(() => {
    const range = this.getPeriodRange(this.periodMode());
    const all = this.transactions();
    if (!range) return all.filter(t => t.status !== 'Cancelled');
    const { start, end } = range;
    return all.filter(t => {
      const d = new Date(t.dueDate);
      return d >= start && d < end && t.status !== 'Cancelled';
    });
  });

  // ---- KPIs de transações ----
  totalRevenue = computed(() =>
    this.periodFiltered().filter(t => t.type === 'Revenue' && t.status === 'Paid').reduce((s, t) => s + t.amount, 0)
  );
  totalExpense = computed(() =>
    this.periodFiltered().filter(t => t.type === 'Expense' && t.status === 'Paid').reduce((s, t) => s + t.amount, 0)
  );
  monthBalance = computed(() => this.totalRevenue() - this.totalExpense());
  pendingCount = computed(() => this.periodFiltered().filter(t => t.status === 'Pending').length);
  overdueCount = computed(() => this.periodFiltered().filter(t => t.isOverdue && t.status === 'Pending').length);

  // ---- KPIs de contas ----
  totalAccountBalance = computed(() =>
    this.accounts().reduce((s, a) => s + a.currentBalance, 0)
  );

  // ---- Metas ----
  activeGoals = computed(() => this.goals().filter(g => !g.isCompleted));
  completedGoalsCount = computed(() => this.goals().filter(g => g.isCompleted).length);
  topGoals = computed(() => [...this.activeGoals()].sort((a, b) => b.progressRate - a.progressRate).slice(0, 3));

  // ---- Transações recentes ----
  recentTransactions = computed(() =>
    [...this.periodFiltered()]
      .sort((a, b) => new Date(b.dueDate).getTime() - new Date(a.dueDate).getTime())
      .slice(0, 6)
  );

  // ---- Gráfico: Despesas por Categoria ----
  private static readonly EXPENSE_PALETTE = [
    '#D85A30', '#993C1D', '#8e24aa', '#d81b60', '#f4511e',
    '#BA7517', '#6d4c41', '#bf360c', '#e53935', '#c0392b',
  ];

  expensePieData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const palette = [this.cssVar('--color-expense'), ...DashboardComponent.EXPENSE_PALETTE.slice(1)];
    const expenses = this.periodFiltered().filter(t => t.type === 'Expense' && t.status === 'Paid');
    const byCategory = new Map<string, number>();
    for (const t of expenses) {
      const key = t.categoryName || 'Sem categoria';
      byCategory.set(key, (byCategory.get(key) ?? 0) + t.amount);
    }
    const entries = [...byCategory.entries()].filter(([, v]) => v > 0).sort((a, b) => b[1] - a[1]);
    return {
      labels: entries.map(([k]) => k),
      datasets: [{
        data: entries.map(([, v]) => v),
        backgroundColor: entries.map((_, i) => palette[i % palette.length]),
      }],
    };
  });

  expensePieOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      tooltip: {
        callbacks: {
          label: (ctx) => {
            const value = Number(ctx.parsed) || 0;
            const total = (ctx.dataset.data as number[]).reduce((s, v) => s + (Number(v) || 0), 0);
            const pct = total > 0 ? ((value / total) * 100).toFixed(1) : '0';
            return `${ctx.label}: ${value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })} (${pct}%)`;
          },
        },
      },
    },
  };

  // ---- Gráfico: Saldo por conta (barras horizontais) ----
  accountBarData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const accs = this.accounts().slice(0, 8);
    return {
      labels: accs.map(a => a.name),
      datasets: [{
        label: 'Saldo atual',
        data: accs.map(a => a.currentBalance),
        backgroundColor: accs.map(a => a.currentBalance >= 0 ? this.cssVar('--color-revenue') : this.cssVar('--color-expense')),
        borderRadius: 4,
      }],
    };
  });

  accountBarOptions: ChartConfiguration['options'] = {
    indexAxis: 'y',
    responsive: true,
    plugins: { legend: { display: false } },
    scales: {
      x: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } },
    },
  };

  ngOnInit(): void {
    const now = new Date();
    forkJoin({
      transactions: this.transactionService.getAll(),
      accounts:     this.accountService.getAll(),
      portfolio:    this.investmentService.getPortfolio(),
      loanSummary:  this.loanService.getSummary(),
      goals:        this.goalService.getAll(),
      netWorth:     this.patrimonyService.getNetWorth(),
      snapshots:    this.patrimonySnapshotService.getAll(),
      planning:     this.planningService.get(now.getFullYear(), now.getMonth() + 1),
    }).subscribe({
      next: (data) => {
        this.transactions.set(data.transactions);
        this.accounts.set(data.accounts);
        this.portfolio.set(data.portfolio);
        this.loanSummary.set(data.loanSummary);
        this.goals.set(data.goals);
        this.netWorth.set(data.netWorth);
        this.snapshots.set(data.snapshots);
        this.planning.set(data.planning);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  setPeriod(value: string): void {
    this.periodMode.set(value as ReturnType<typeof this.periodMode>);
  }

  onCustomDateChange(): void {
    this.customStart.set(this.customStartCtrl.value);
    this.customEnd.set(this.customEndCtrl.value);
  }

  periodLabel(): string {
    const map: Record<string, string> = {
      thisMonth:  'Este mês',
      lastMonth:  'Mês passado',
      last30Days: 'Últimos 30 dias',
      thisYear:   'Este ano',
      all:        'Tudo',
      custom:     'Personalizado',
    };
    return map[this.periodMode()] ?? '';
  }

  private getPeriodRange(mode: string): { start: Date; end: Date } | null {
    const now = new Date();
    switch (mode) {
      case 'thisMonth':  return { start: new Date(now.getFullYear(), now.getMonth(), 1), end: new Date(now.getFullYear(), now.getMonth() + 1, 1) };
      case 'lastMonth':  return { start: new Date(now.getFullYear(), now.getMonth() - 1, 1), end: new Date(now.getFullYear(), now.getMonth(), 1) };
      case 'last30Days': {
        const s = new Date(now); s.setDate(s.getDate() - 30); s.setHours(0, 0, 0, 0);
        const e = new Date(now); e.setDate(e.getDate() + 1); e.setHours(0, 0, 0, 0);
        return { start: s, end: e };
      }
      case 'thisYear':   return { start: new Date(now.getFullYear(), 0, 1), end: new Date(now.getFullYear() + 1, 0, 1) };
      case 'custom': {
        const s = this.customStart();
        const e = this.customEnd();
        if (!s || !e) return null;
        const end = new Date(e); end.setHours(23, 59, 59, 999);
        return { start: s, end };
      }
      default:           return null;
    }
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = { Pending: 'Pendente', Paid: 'Pago', Cancelled: 'Cancelado' };
    return map[status] ?? status;
  }

  goalProgressColor(rate: number): string {
    if (rate >= 100) return '#2e7d32';
    if (rate >= 60)  return '#1976d2';
    if (rate >= 30)  return '#ff9800';
    return '#e53935';
  }
}
