import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartConfiguration } from 'chart.js';
import { Chart, registerables } from 'chart.js';

import { TransactionService } from '../../core/services/transaction.service';
import { ThemeService } from '../../core/services/theme.service';
import { Transaction } from '../../core/models/transaction.model';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';

Chart.register(...registerables);

type PeriodMode = 'thisMonth' | 'lastMonth' | 'last3Months' | 'last6Months' | 'thisYear' | 'custom';

const MONTHS_PT = ['Jan','Fev','Mar','Abr','Mai','Jun','Jul','Ago','Set','Out','Nov','Dez'];
const EXPENSE_PALETTE = ['#D85A30','#993C1D','#8e24aa','#d81b60','#f4511e','#BA7517','#6d4c41','#bf360c','#e53935','#c0392b'];

@Component({
  selector: 'app-cash-flow',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatFormFieldModule, MatSelectModule, MatInputModule, MatDatepickerModule,
    BaseChartDirective, DashboardTabsComponent,
  ],
  templateUrl: './cash-flow.component.html',
  styleUrl:    './cash-flow.component.scss',
})
export class CashFlowComponent implements OnInit {
  private readonly transactionService = inject(TransactionService);
  private readonly themeService       = inject(ThemeService);

  private cssVar(name: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim();
  }

  loading      = signal(true);
  transactions = signal<Transaction[]>([]);

  periodMode = signal<PeriodMode>('last6Months');
  showCustomDates = computed(() => this.periodMode() === 'custom');

  customStartCtrl = new FormControl<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
  customEndCtrl   = new FormControl<Date | null>(new Date());
  customStart     = signal<Date | null>(this.customStartCtrl.value);
  customEnd       = signal<Date | null>(this.customEndCtrl.value);

  private get periodRange(): { start: Date; end: Date } {
    const now = new Date();
    switch (this.periodMode()) {
      case 'thisMonth':   return { start: new Date(now.getFullYear(), now.getMonth(), 1), end: new Date(now.getFullYear(), now.getMonth() + 1, 1) };
      case 'lastMonth':   return { start: new Date(now.getFullYear(), now.getMonth() - 1, 1), end: new Date(now.getFullYear(), now.getMonth(), 1) };
      case 'last3Months': return { start: new Date(now.getFullYear(), now.getMonth() - 2, 1), end: new Date(now.getFullYear(), now.getMonth() + 1, 1) };
      case 'last6Months': return { start: new Date(now.getFullYear(), now.getMonth() - 5, 1), end: new Date(now.getFullYear(), now.getMonth() + 1, 1) };
      case 'thisYear':    return { start: new Date(now.getFullYear(), 0, 1), end: new Date(now.getFullYear() + 1, 0, 1) };
      case 'custom': {
        const s = this.customStart() ?? new Date(now.getFullYear(), now.getMonth(), 1);
        const e = this.customEnd() ?? now;
        const end = new Date(e); end.setHours(23, 59, 59, 999);
        return { start: s, end };
      }
    }
  }

  private filtered = computed(() => {
    const { start, end } = this.periodRange;
    return this.transactions().filter(t => {
      const d = new Date(t.dueDate);
      return d >= start && d < end && t.status !== 'Cancelled';
    });
  });

  // ── KPIs ──────────────────────────────────────────────────────────────
  totalRevenue = computed(() =>
    this.filtered().filter(t => t.type === 'Revenue' && t.status === 'Paid').reduce((s, t) => s + t.amount, 0)
  );
  totalExpense = computed(() =>
    this.filtered().filter(t => t.type === 'Expense' && t.status === 'Paid').reduce((s, t) => s + t.amount, 0)
  );
  monthlyBalance = computed(() => this.totalRevenue() - this.totalExpense());
  savingsRate    = computed(() => {
    const r = this.totalRevenue();
    return r > 0 ? ((r - this.totalExpense()) / r) * 100 : 0;
  });

  // ── Monthly buckets ───────────────────────────────────────────────────
  private monthlyData = computed(() => {
    const { start, end } = this.periodRange;
    const buckets = new Map<string, { revenue: number; expense: number }>();
    let cur = new Date(start.getFullYear(), start.getMonth(), 1);
    while (cur < end) {
      const key = `${cur.getFullYear()}-${cur.getMonth()}`;
      buckets.set(key, { revenue: 0, expense: 0 });
      cur = new Date(cur.getFullYear(), cur.getMonth() + 1, 1);
    }
    for (const t of this.filtered()) {
      if (t.status !== 'Paid') continue;
      const d = new Date(t.dueDate);
      const key = `${d.getFullYear()}-${d.getMonth()}`;
      const bucket = buckets.get(key);
      if (!bucket) continue;
      if (t.type === 'Revenue') bucket.revenue += t.amount;
      if (t.type === 'Expense') bucket.expense += t.amount;
    }
    return buckets;
  });

  private monthLabels = computed(() =>
    [...this.monthlyData().keys()].map(k => {
      const [y, m] = k.split('-').map(Number);
      return `${MONTHS_PT[m]}/${String(y).slice(2)}`;
    })
  );

  // ── Gráfico: Receita x Despesa (barras) ───────────────────────────────
  revExpBarData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const values = [...this.monthlyData().values()];
    return {
      labels: this.monthLabels(),
      datasets: [
        { label: 'Receita', data: values.map(v => v.revenue), backgroundColor: this.cssVar('--color-revenue'), borderRadius: 4 },
        { label: 'Despesa', data: values.map(v => v.expense), backgroundColor: this.cssVar('--color-expense'), borderRadius: 4 },
      ],
    };
  });

  revExpBarOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico: Evolução do Saldo (linha) ────────────────────────────────
  balanceLineData = computed<ChartData<'line'>>(() => {
    void this.themeService.current();
    const values = [...this.monthlyData().values()];
    const balances = values.map(v => v.revenue - v.expense);
    const color = this.cssVar('--color-primary') || '#1976d2';
    return {
      labels: this.monthLabels(),
      datasets: [{
        label: 'Saldo Mensal',
        data: balances,
        borderColor: color,
        backgroundColor: color + '33',
        fill: true,
        tension: 0.3,
        pointRadius: 4,
      }],
    };
  });

  lineOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { display: false } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico: Evolução Receitas e Despesas (linha dupla) ───────────────
  revExpLineData = computed<ChartData<'line'>>(() => {
    void this.themeService.current();
    const values = [...this.monthlyData().values()];
    return {
      labels: this.monthLabels(),
      datasets: [
        {
          label: 'Receitas',
          data: values.map(v => v.revenue),
          borderColor: this.cssVar('--color-revenue'),
          backgroundColor: 'transparent',
          tension: 0.3, pointRadius: 4,
        },
        {
          label: 'Despesas',
          data: values.map(v => v.expense),
          borderColor: this.cssVar('--color-expense'),
          backgroundColor: 'transparent',
          tension: 0.3, pointRadius: 4,
        },
      ],
    };
  });

  revExpLineOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico: Despesas por Categoria (doughnut) ────────────────────────
  expensePieData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const palette = [this.cssVar('--color-expense'), ...EXPENSE_PALETTE.slice(1)];
    const expenses = this.filtered().filter(t => t.type === 'Expense' && t.status === 'Paid');
    const byCategory = new Map<string, number>();
    for (const t of expenses) {
      const key = t.categoryName || 'Sem categoria';
      byCategory.set(key, (byCategory.get(key) ?? 0) + t.amount);
    }
    const entries = [...byCategory.entries()].filter(([, v]) => v > 0).sort((a, b) => b[1] - a[1]);
    return {
      labels: entries.map(([k]) => k),
      datasets: [{ data: entries.map(([, v]) => v), backgroundColor: entries.map((_, i) => palette[i % palette.length]) }],
    };
  });

  expensePieOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      tooltip: {
        callbacks: {
          label: (ctx) => {
            const v = Number(ctx.parsed) || 0;
            const total = (ctx.dataset.data as number[]).reduce((s, x) => s + (Number(x) || 0), 0);
            const pct = total > 0 ? ((v / total) * 100).toFixed(1) : '0';
            return `${ctx.label}: ${v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })} (${pct}%)`;
          },
        },
      },
    },
  };

  ngOnInit(): void {
    this.transactionService.getAll().subscribe({
      next: (txs) => { this.transactions.set(txs); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  setPeriod(value: string): void { this.periodMode.set(value as PeriodMode); }

  onCustomDateChange(): void {
    this.customStart.set(this.customStartCtrl.value);
    this.customEnd.set(this.customEndCtrl.value);
  }

  periodLabel(): string {
    const map: Record<string, string> = {
      thisMonth: 'Este mês', lastMonth: 'Mês passado',
      last3Months: 'Últimos 3 meses', last6Months: 'Últimos 6 meses',
      thisYear: 'Este ano', custom: 'Personalizado',
    };
    return map[this.periodMode()] ?? '';
  }

  savingsRateColor(): string {
    const r = this.savingsRate();
    if (r >= 20) return '#2e7d32';
    if (r >= 10) return '#f57c00';
    return '#c62828';
  }
}
