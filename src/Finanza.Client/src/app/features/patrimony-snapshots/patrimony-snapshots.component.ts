import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import {
  Chart,
  LineElement, PointElement, LineController,
  CategoryScale, LinearScale, Filler, Tooltip, Legend,
} from 'chart.js';

import { PatrimonySnapshotService } from '../../core/services/patrimony-snapshot.service';
import { PatrimonySnapshot } from '../../core/models/patrimony-snapshot.model';

Chart.register(LineElement, PointElement, LineController, CategoryScale, LinearScale, Filler, Tooltip, Legend);

@Component({
  selector: 'app-patrimony-snapshots',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DatePipe,
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatSnackBarModule,
    BaseChartDirective,
  ],
  templateUrl: './patrimony-snapshots.component.html',
  styleUrl:    './patrimony-snapshots.component.scss',
})
export class PatrimonySnapshotsComponent implements OnInit {
  private readonly service  = inject(PatrimonySnapshotService);
  private readonly snackBar = inject(MatSnackBar);

  loading         = signal(true);
  takingSnapshot  = signal(false);
  snapshots       = signal<PatrimonySnapshot[]>([]);

  chartData     = signal<ChartData<'line'>>({ labels: [], datasets: [] });
  chartOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: {
      legend: { position: 'top' },
      tooltip: {
        callbacks: {
          label: ctx => `R$ ${(ctx.parsed.y ?? 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`,
        },
      },
    },
    scales: {
      y: {
        ticks: {
          callback: v => `R$ ${Number(v).toLocaleString('pt-BR', { minimumFractionDigits: 0 })}`,
        },
      },
    },
  };

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: snaps => {
        const sorted = [...snaps].sort((a, b) => a.date.localeCompare(b.date));
        this.snapshots.set(sorted);
        this.buildChart(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  takeSnapshot(): void {
    this.takingSnapshot.set(true);
    this.service.create().subscribe({
      next: () => {
        this.takingSnapshot.set(false);
        this.snackBar.open('Snapshot registrado!', 'OK', { duration: 3000 });
        this.load();
      },
      error: () => this.takingSnapshot.set(false),
    });
  }

  private buildChart(snaps: PatrimonySnapshot[]): void {
    const labels = snaps.map(s => new Date(s.date).toLocaleDateString('pt-BR'));
    this.chartData.set({
      labels,
      datasets: [
        {
          label: 'Patrimônio Líquido',
          data: snaps.map(s => s.netWorth),
          borderColor: '#4caf50',
          backgroundColor: 'rgba(76,175,80,0.1)',
          fill: true,
          tension: 0.3,
        },
        {
          label: 'Ativos',
          data: snaps.map(s => s.totalAssets),
          borderColor: '#2196f3',
          backgroundColor: 'rgba(33,150,243,0.05)',
          fill: false,
          tension: 0.3,
        },
        {
          label: 'Passivos',
          data: snaps.map(s => s.totalLiabilities),
          borderColor: '#f44336',
          backgroundColor: 'rgba(244,67,54,0.05)',
          fill: false,
          tension: 0.3,
        },
      ],
    });
  }
}
