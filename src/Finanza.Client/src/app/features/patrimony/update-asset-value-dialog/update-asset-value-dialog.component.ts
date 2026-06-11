import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { Chart, LineElement, PointElement, LineController, CategoryScale, LinearScale, Tooltip } from 'chart.js';

import { AssetValueHistoryService } from '../../../core/services/asset-value-history.service';
import { AssetValueHistory } from '../../../core/models/asset-value-history.model';
import { CurrencyMaskDirective } from '../../../core/directives/currency-mask.directive';

Chart.register(LineElement, PointElement, LineController, CategoryScale, LinearScale, Tooltip);

export interface UpdateAssetValueDialogData {
  assetId: string;
  assetName: string;
  currentValue: number;
}

@Component({
  selector: 'app-update-asset-value-dialog',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatDialogModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    BaseChartDirective, CurrencyMaskDirective,
  ],
  templateUrl: './update-asset-value-dialog.component.html',
})
export class UpdateAssetValueDialogComponent {
  private readonly service  = inject(AssetValueHistoryService);
  private readonly fb       = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<UpdateAssetValueDialogComponent>);
  readonly data = inject<UpdateAssetValueDialogData>(MAT_DIALOG_DATA);

  saving   = signal(false);
  history  = signal<AssetValueHistory[]>([]);
  chartData = signal<ChartData<'line'>>({ labels: [], datasets: [] });

  chartOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: { legend: { display: false } },
    scales: {
      y: { ticks: { callback: v => `R$ ${Number(v).toLocaleString('pt-BR', { minimumFractionDigits: 0 })}` } },
    },
  };

  form = this.fb.group({
    value: [this.data.currentValue, [Validators.required, Validators.min(0)]],
    date:  [new Date().toISOString().substring(0, 10), Validators.required],
  });

  constructor() {
    this.loadHistory();
  }

  loadHistory(): void {
    this.service.getHistory(this.data.assetId).subscribe(h => {
      const sorted = [...h].sort((a, b) => a.date.localeCompare(b.date));
      this.history.set(sorted);
      this.chartData.set({
        labels: sorted.map(e => new Date(e.date).toLocaleDateString('pt-BR')),
        datasets: [{
          data: sorted.map(e => e.value),
          borderColor: '#2196f3',
          backgroundColor: 'rgba(33,150,243,0.1)',
          fill: true,
          tension: 0.3,
        }],
      });
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const { value, date } = this.form.getRawValue();
    this.service.updateValue(this.data.assetId, { value: value!, date: date! }).subscribe({
      next: () => { this.saving.set(false); this.dialogRef.close(true); },
      error: () => this.saving.set(false),
    });
  }

  cancel(): void { this.dialogRef.close(false); }
}
