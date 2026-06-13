import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartConfiguration } from 'chart.js';
import { Chart, registerables } from 'chart.js';
import { forkJoin } from 'rxjs';

Chart.register(...registerables);

import { PatrimonyService } from '../../core/services/patrimony.service';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';
import { PatrimonySnapshotService } from '../../core/services/patrimony-snapshot.service';
import { ThemeService } from '../../core/services/theme.service';
import {
  Asset, Liability, NetWorth,
  ASSET_TYPE_LABELS, LIABILITY_TYPE_LABELS,
  ASSET_TYPE_OPTIONS, LIABILITY_TYPE_OPTIONS,
} from '../../core/models/patrimony.model';
import { PatrimonySnapshot } from '../../core/models/patrimony-snapshot.model';
import {
  PatrimonyItemFormComponent,
  PatrimonyItemFormData,
} from './patrimony-item-form/patrimony-item-form.component';
import {
  UpdateAssetValueDialogComponent,
  UpdateAssetValueDialogData,
} from './update-asset-value-dialog/update-asset-value-dialog.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-patrimony',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DatePipe, MatCardModule, MatButtonModule, MatIconModule,
    MatDividerModule, MatProgressSpinnerModule, MatTooltipModule, DashboardTabsComponent,
    MatDialogModule, MatSnackBarModule, BaseChartDirective,
  ],
  templateUrl: './patrimony.component.html',
  styleUrl:    './patrimony.component.scss',
})
export class PatrimonyComponent implements OnInit {
  private readonly service          = inject(PatrimonyService);
  private readonly snapshotService  = inject(PatrimonySnapshotService);
  private readonly themeService     = inject(ThemeService);
  private readonly dialog           = inject(MatDialog);
  private readonly snackBar         = inject(MatSnackBar);

  private cssVar(name: string): string {
    return getComputedStyle(document.body).getPropertyValue(name).trim();
  }

  takingSnapshot    = signal(false);
  loading           = signal(true);
  netWorth          = signal<NetWorth | null>(null);
  snapshots         = signal<PatrimonySnapshot[]>([]);
  expandedLiability = signal<string | null>(null);

  readonly assetLabels     = ASSET_TYPE_LABELS;
  readonly liabilityLabels = LIABILITY_TYPE_LABELS;

  hasSnapshots = computed(() => this.snapshots().length >= 2);

  // ── Gráfico 1: Evolução Patrimonial (linha) ───────────────────────────
  evolutionLineData = computed<ChartData<'line'>>(() => {
    void this.themeService.current();
    const snaps = [...this.snapshots()].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
    const labels = snaps.map(s => {
      const d = new Date(s.date);
      return `${d.getDate().toString().padStart(2,'0')}/${(d.getMonth()+1).toString().padStart(2,'0')}/${d.getFullYear()}`;
    });
    const primary = this.cssVar('--color-primary') || '#1976d2';
    const revenue = this.cssVar('--color-revenue') || '#2e7d32';
    const expense = this.cssVar('--color-expense') || '#c62828';
    return {
      labels,
      datasets: [
        { label: 'Patrimônio Líquido', data: snaps.map(s => s.netWorth),       borderColor: primary, backgroundColor: primary + '22', fill: true, tension: 0.3, pointRadius: 4 },
        { label: 'Ativos',             data: snaps.map(s => s.totalAssets),     borderColor: revenue, backgroundColor: 'transparent', tension: 0.3, pointRadius: 3 },
        { label: 'Passivos',           data: snaps.map(s => s.totalLiabilities),borderColor: expense, backgroundColor: 'transparent', tension: 0.3, pointRadius: 3 },
      ],
    };
  });

  evolutionLineOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico 2: Ativos x Passivos por snapshot (barras) ───────────────
  assetsVsLiabilitiesData = computed<ChartData<'bar'>>(() => {
    void this.themeService.current();
    const snaps = [...this.snapshots()].sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
    const labels = snaps.map(s => {
      const d = new Date(s.date);
      return `${d.getDate().toString().padStart(2,'0')}/${(d.getMonth()+1).toString().padStart(2,'0')}`;
    });
    return {
      labels,
      datasets: [
        { label: 'Ativos',   data: snaps.map(s => s.totalAssets),     backgroundColor: this.cssVar('--color-revenue'), borderRadius: 4 },
        { label: 'Passivos', data: snaps.map(s => s.totalLiabilities), backgroundColor: this.cssVar('--color-expense'), borderRadius: 4 },
      ],
    };
  });

  barOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'top' } },
    scales: { y: { ticks: { callback: (v) => `R$ ${Number(v).toLocaleString('pt-BR')}` } } },
  };

  // ── Gráfico 3: Distribuição Patrimonial atual (doughnut) ──────────────
  distributionData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const nw = this.netWorth();
    if (!nw) return { labels: [], datasets: [{ data: [] }] };
    const byType = new Map<string, number>();
    for (const a of nw.assets) {
      const label = ASSET_TYPE_LABELS[a.type] ?? a.type;
      byType.set(label, (byType.get(label) ?? 0) + a.value);
    }
    const entries = [...byType.entries()].sort((a, b) => b[1] - a[1]);
    const palette = ['#1565c0','#1976d2','#1e88e5','#42a5f5','#90caf9','#0d47a1','#1a237e','#283593','#303f9f','#3949ab'];
    return {
      labels: entries.map(([k]) => k),
      datasets: [{ data: entries.map(([, v]) => v), backgroundColor: palette }],
    };
  });

  doughnutOptions: ChartConfiguration['options'] = {
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

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    forkJoin({
      netWorth:  this.service.getNetWorth(),
      snapshots: this.snapshotService.getAll(),
    }).subscribe({
      next: (data) => {
        this.netWorth.set(data.netWorth);
        this.snapshots.set(data.snapshots);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  assetIcon(type: string): string {
    return ASSET_TYPE_OPTIONS.find(o => o.label === this.assetLabels[type])?.icon ?? 'category';
  }

  liabilityIcon(type: string): string {
    return LIABILITY_TYPE_OPTIONS.find(o => o.label === this.liabilityLabels[type])?.icon ?? 'category';
  }

  openAddAsset(): void {
    this.openForm({ kind: 'asset' }, 'Ativo cadastrado!');
  }

  openEditAsset(item: Asset): void {
    this.openForm({ kind: 'asset', item }, 'Ativo atualizado!');
  }

  openAddLiability(): void {
    this.openForm({ kind: 'liability' }, 'Passivo cadastrado!');
  }

  openEditLiability(item: Liability): void {
    this.openForm({ kind: 'liability', item }, 'Passivo atualizado!');
  }

  private openForm(data: PatrimonyItemFormData, successMsg: string): void {
    const ref = this.dialog.open(PatrimonyItemFormComponent, {
      width: '480px', disableClose: true, data,
    });
    ref.afterClosed().subscribe(result => {
      if (result) { this.snackBar.open(successMsg, 'OK', { duration: 3000 }); this.load(); }
    });
  }

  openUpdateValue(item: Asset): void {
    const data: UpdateAssetValueDialogData = {
      assetId:      item.id,
      assetName:    item.name,
      currentValue: item.value,
    };
    const ref = this.dialog.open(UpdateAssetValueDialogComponent, {
      width: '500px', disableClose: true, data,
    });
    ref.afterClosed().subscribe(result => {
      if (result) { this.snackBar.open('Valor atualizado!', 'OK', { duration: 3000 }); this.load(); }
    });
  }

  deleteAsset(item: Asset): void {
    this.confirmDelete(`ativo "${item.name}"`, () =>
      this.service.deleteAsset(item.id).subscribe({ next: () => this.load() })
    );
  }

  deleteLiability(item: Liability): void {
    this.confirmDelete(`passivo "${item.name}"`, () =>
      this.service.deleteLiability(item.id).subscribe({ next: () => this.load() })
    );
  }

  toggleInstallments(liabilityId: string): void {
    this.expandedLiability.set(this.expandedLiability() === liabilityId ? null : liabilityId);
  }

  payInstallment(installmentId: string, liabilityId: string): void {
    const today = new Date().toISOString();
    this.service.payLiabilityInstallment(installmentId, { paidAt: today }).subscribe({
      next: () => {
        this.snackBar.open('Parcela paga!', 'OK', { duration: 3000 });
        this.load();
        this.expandedLiability.set(liabilityId);
      },
    });
  }

  unpayInstallment(installmentId: string, liabilityId: string): void {
    this.service.unpayLiabilityInstallment(installmentId).subscribe({
      next: () => {
        this.snackBar.open('Pagamento desfeito.', 'OK', { duration: 3000 });
        this.load();
        this.expandedLiability.set(liabilityId);
      },
    });
  }

  takeSnapshot(): void {
    this.takingSnapshot.set(true);
    this.snapshotService.create().subscribe({
      next: () => {
        this.takingSnapshot.set(false);
        this.snackBar.open('Snapshot registrado!', 'OK', { duration: 3000 });
      },
      error: () => this.takingSnapshot.set(false),
    });
  }

  private confirmDelete(label: string, onConfirm: () => void): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Excluir', message: `Tem certeza que deseja excluir o ${label}?`, confirmLabel: 'Excluir' },
    });
    ref.afterClosed().subscribe(ok => { if (ok) onConfirm(); });
  }
}
