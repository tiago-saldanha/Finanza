import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions, ChartConfiguration } from 'chart.js';
import { Chart, registerables } from 'chart.js';

import { InvestmentService, InvestmentRequest } from '../../core/services/investment.service';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';
import { ThemeService } from '../../core/services/theme.service';
import {
  Investment, InvestmentPortfolio,
  INVESTMENT_TYPE_OPTIONS, INVESTMENT_TYPE_LABELS,
} from '../../core/models/investment.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyMaskDirective } from '../../core/directives/currency-mask.directive';

Chart.register(...registerables);

// Taxa Selic de referência (% a.a.) — atualizar conforme necessário
const SELIC_RATE_AA = 10.5;

@Component({
  selector: 'app-investments',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatTooltipModule, MatDividerModule, DashboardTabsComponent,
    BaseChartDirective, CurrencyMaskDirective,
  ],
  templateUrl: './investments.component.html',
  styleUrl:    './investments.component.scss',
})
export class InvestmentsComponent implements OnInit {
  private readonly service      = inject(InvestmentService);
  private readonly themeService = inject(ThemeService);
  private readonly dialog       = inject(MatDialog);
  private readonly snackBar     = inject(MatSnackBar);
  private readonly fb           = inject(FormBuilder);

  private cssVar(name: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim();
  }

  loading   = signal(true);
  saving    = signal(false);
  portfolio = signal<InvestmentPortfolio | null>(null);
  editing   = signal<Investment | null>(null);
  showForm  = signal(false);

  readonly typeOptions  = INVESTMENT_TYPE_OPTIONS;
  readonly typeLabels   = INVESTMENT_TYPE_LABELS;
  readonly selicRateAa  = SELIC_RATE_AA;

  // ── Benchmark Selic ───────────────────────────────────────────────────
  selicReturn = computed(() => {
    const p = this.portfolio();
    if (!p || p.totalInvested === 0) return 0;
    return p.totalInvested * (SELIC_RATE_AA / 100);
  });

  beatingsSelic = computed(() => {
    const p = this.portfolio();
    if (!p) return false;
    return p.totalReturnRate >= SELIC_RATE_AA;
  });

  // ── Gráfico: Alocação por tipo (doughnut) ─────────────────────────────
  chartData     = signal<ChartData<'doughnut'>>({ labels: [], datasets: [] });
  chartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    plugins: {
      legend: { position: 'right' },
      tooltip: {
        callbacks: {
          label: ctx => ` ${ctx.label}: ${(ctx.parsed ?? 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}%`,
        },
      },
    },
  };

  // ── Gráfico: Crescimento por investimento (barras) ────────────────────
  growthBarData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const investments = this.portfolio()?.investments ?? [];
    return {
      labels: investments.map(i => i.name.length > 16 ? i.name.slice(0, 14) + '…' : i.name),
      datasets: [
        { label: 'Investido',    data: investments.map(i => i.investedAmount), backgroundColor: '#90caf9', borderRadius: 4 },
        { label: 'Valor Atual',  data: investments.map(i => i.currentValue),   backgroundColor: this.cssVar('--color-revenue') || '#2e7d32', borderRadius: 4 },
      ],
    };
  });

  growthBarOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico: Carteira vs Selic (barras horizontais) ───────────────────
  benchmarkBarData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const p = this.portfolio();
    if (!p) return { labels: [], datasets: [{ data: [] }] };
    const carteiraRate = p.totalReturnRate;
    const selicRate    = SELIC_RATE_AA;
    const beating      = carteiraRate >= selicRate;
    return {
      labels: ['Selic (ref.)', 'Minha Carteira'],
      datasets: [{
        label: 'Retorno (%)',
        data: [selicRate, carteiraRate],
        backgroundColor: ['#90caf9', beating ? (this.cssVar('--color-revenue') || '#2e7d32') : (this.cssVar('--color-expense') || '#c62828')],
        borderRadius: 6,
      }],
    };
  });

  benchmarkBarOptions: ChartConfiguration['options'] = {
    indexAxis: 'y',
    responsive: true,
    plugins: { legend: { display: false } },
    scales: { x: { ticks: { callback: (v) => `${v}%` } } },
  };

  form = this.fb.group({
    name:           ['', Validators.required],
    type:           [0,  Validators.required],
    investedAmount: [0,  [Validators.required, Validators.min(0)]],
    currentValue:   [0,  [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.service.getPortfolio().subscribe({
      next: p => { this.portfolio.set(p); this.buildChart(p); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openAdd(): void {
    this.editing.set(null);
    this.form.reset({ name: '', type: 0, investedAmount: 0, currentValue: 0 });
    this.showForm.set(true);
  }

  openEdit(inv: Investment): void {
    this.editing.set(inv);
    const typeValue = INVESTMENT_TYPE_OPTIONS.find(o => o.label === this.typeLabels[inv.type])?.value ?? 0;
    this.form.setValue({ name: inv.name, type: typeValue, investedAmount: inv.investedAmount, currentValue: inv.currentValue });
    this.showForm.set(true);
  }

  cancelForm(): void { this.showForm.set(false); }

  submit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const req = this.form.getRawValue() as InvestmentRequest;
    const editing = this.editing();
    const op = editing
      ? this.service.update(editing.id, req)
      : this.service.create(req);
    op.subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.snackBar.open(editing ? 'Investimento atualizado!' : 'Investimento cadastrado!', 'OK', { duration: 3000 });
        this.load();
      },
      error: () => this.saving.set(false),
    });
  }

  delete(inv: Investment): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Excluir', message: `Excluir "${inv.name}"?`, confirmLabel: 'Excluir' },
    });
    ref.afterClosed().subscribe(ok => {
      if (ok) this.service.delete(inv.id).subscribe({ next: () => this.load() });
    });
  }

  typeIcon(type: string): string {
    return INVESTMENT_TYPE_OPTIONS.find(o => o.label === this.typeLabels[type])?.icon ?? 'category';
  }

  private buildChart(p: InvestmentPortfolio): void {
    if (!p.allocations.length) { this.chartData.set({ labels: [], datasets: [] }); return; }
    this.chartData.set({
      labels:   p.allocations.map(a => a.type),
      datasets: [{ data: p.allocations.map(a => a.percentage), borderWidth: 1 }],
    });
  }
}
