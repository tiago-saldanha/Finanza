import { Component, OnInit, inject, signal } from '@angular/core';
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
import { ChartData, ChartOptions } from 'chart.js';
import { Chart, ArcElement, DoughnutController, Tooltip, Legend } from 'chart.js';

import { InvestmentService, InvestmentRequest } from '../../core/services/investment.service';
import {
  Investment, InvestmentPortfolio,
  INVESTMENT_TYPE_OPTIONS, INVESTMENT_TYPE_LABELS,
} from '../../core/models/investment.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyMaskDirective } from '../../core/directives/currency-mask.directive';

Chart.register(ArcElement, DoughnutController, Tooltip, Legend);

@Component({
  selector: 'app-investments',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatTooltipModule, MatDividerModule,
    BaseChartDirective, CurrencyMaskDirective,
  ],
  templateUrl: './investments.component.html',
  styleUrl:    './investments.component.scss',
})
export class InvestmentsComponent implements OnInit {
  private readonly service  = inject(InvestmentService);
  private readonly dialog   = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb       = inject(FormBuilder);

  loading   = signal(true);
  saving    = signal(false);
  portfolio = signal<InvestmentPortfolio | null>(null);
  editing   = signal<Investment | null>(null);
  showForm  = signal(false);

  readonly typeOptions = INVESTMENT_TYPE_OPTIONS;
  readonly typeLabels  = INVESTMENT_TYPE_LABELS;

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
