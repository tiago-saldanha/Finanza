import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
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

import { PatrimonyService } from '../../core/services/patrimony.service';
import { PatrimonySnapshotService } from '../../core/services/patrimony-snapshot.service';
import { ThemeService } from '../../core/services/theme.service';
import {
  Asset, Liability, NetWorth,
  ASSET_TYPE_LABELS, LIABILITY_TYPE_LABELS,
  ASSET_TYPE_OPTIONS, LIABILITY_TYPE_OPTIONS,
} from '../../core/models/patrimony.model';
import {
  PatrimonyItemFormComponent,
  PatrimonyItemFormData,
} from '../patrimony/patrimony-item-form/patrimony-item-form.component';
import {
  UpdateAssetValueDialogComponent,
  UpdateAssetValueDialogData,
} from '../patrimony/update-asset-value-dialog/update-asset-value-dialog.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';

Chart.register(...registerables);

const ASSET_PALETTE     = ['#1565c0','#1976d2','#1e88e5','#42a5f5','#90caf9','#bbdefb'];
const LIABILITY_PALETTE = ['#b71c1c','#c62828','#d32f2f','#e53935','#ef5350','#ef9a9a'];

@Component({
  selector: 'app-assets-liabilities',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe,
    MatCardModule, MatButtonModule, MatIconModule, MatDividerModule,
    MatProgressSpinnerModule, MatTooltipModule, MatDialogModule, MatSnackBarModule,
    BaseChartDirective, DashboardTabsComponent,
  ],
  templateUrl: './assets-liabilities.component.html',
  styleUrl:    './assets-liabilities.component.scss',
})
export class AssetsLiabilitiesComponent implements OnInit {
  private readonly service         = inject(PatrimonyService);
  private readonly snapshotService = inject(PatrimonySnapshotService);
  private readonly dialog          = inject(MatDialog);
  private readonly snackBar        = inject(MatSnackBar);
  private readonly themeService    = inject(ThemeService);

  loading        = signal(true);
  takingSnapshot = signal(false);
  netWorth       = signal<NetWorth | null>(null);

  readonly assetLabels     = ASSET_TYPE_LABELS;
  readonly liabilityLabels = LIABILITY_TYPE_LABELS;

  debtRatio = computed(() => {
    const nw = this.netWorth();
    if (!nw || nw.totalAssets === 0) return 0;
    return (nw.totalLiabilities / nw.totalAssets) * 100;
  });

  assetChartData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const assets = this.netWorth()?.assets ?? [];
    const byType = new Map<string, number>();
    for (const a of assets) {
      const label = ASSET_TYPE_LABELS[a.type] ?? a.type;
      byType.set(label, (byType.get(label) ?? 0) + a.value);
    }
    const entries = [...byType.entries()].sort((a, b) => b[1] - a[1]);
    return {
      labels: entries.map(([k]) => k),
      datasets: [{ data: entries.map(([, v]) => v), backgroundColor: ASSET_PALETTE }],
    };
  });

  liabilityChartData = computed<ChartData<'doughnut'>>(() => {
    void this.themeService.current();
    const liabilities = this.netWorth()?.liabilities ?? [];
    const byType = new Map<string, number>();
    for (const l of liabilities) {
      const label = LIABILITY_TYPE_LABELS[l.type] ?? l.type;
      byType.set(label, (byType.get(label) ?? 0) + l.value);
    }
    const entries = [...byType.entries()].sort((a, b) => b[1] - a[1]);
    return {
      labels: entries.map(([k]) => k),
      datasets: [{ data: entries.map(([, v]) => v), backgroundColor: LIABILITY_PALETTE }],
    };
  });

  readonly doughnutOptions: ChartConfiguration['options'] = {
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
    this.service.getNetWorth().subscribe({
      next: (nw) => { this.netWorth.set(nw); this.loading.set(false); },
      error: ()  => this.loading.set(false),
    });
  }

  assetIcon(type: string): string {
    return ASSET_TYPE_OPTIONS.find(o => o.label === ASSET_TYPE_LABELS[type])?.icon ?? 'category';
  }

  liabilityIcon(type: string): string {
    return LIABILITY_TYPE_OPTIONS.find(o => o.label === LIABILITY_TYPE_LABELS[type])?.icon ?? 'category';
  }

  openAddAsset(): void      { this.openForm({ kind: 'asset' }, 'Ativo cadastrado!'); }
  openEditAsset(item: Asset): void  { this.openForm({ kind: 'asset', item }, 'Ativo atualizado!'); }
  openAddLiability(): void  { this.openForm({ kind: 'liability' }, 'Passivo cadastrado!'); }
  openEditLiability(item: Liability): void { this.openForm({ kind: 'liability', item }, 'Passivo atualizado!'); }

  private openForm(data: PatrimonyItemFormData, msg: string): void {
    const ref = this.dialog.open(PatrimonyItemFormComponent, { width: '480px', disableClose: true, data });
    ref.afterClosed().subscribe(r => { if (r) { this.snackBar.open(msg, 'OK', { duration: 3000 }); this.load(); } });
  }

  openUpdateValue(item: Asset): void {
    const data: UpdateAssetValueDialogData = { assetId: item.id, assetName: item.name, currentValue: item.value };
    const ref = this.dialog.open(UpdateAssetValueDialogComponent, { width: '500px', disableClose: true, data });
    ref.afterClosed().subscribe(r => { if (r) { this.snackBar.open('Valor atualizado!', 'OK', { duration: 3000 }); this.load(); } });
  }

  deleteAsset(item: Asset): void {
    this.confirmDelete(`ativo "${item.name}"`, () => this.service.deleteAsset(item.id).subscribe({ next: () => this.load() }));
  }

  deleteLiability(item: Liability): void {
    this.confirmDelete(`passivo "${item.name}"`, () => this.service.deleteLiability(item.id).subscribe({ next: () => this.load() }));
  }

  takeSnapshot(): void {
    this.takingSnapshot.set(true);
    this.snapshotService.create().subscribe({
      next: () => { this.takingSnapshot.set(false); this.snackBar.open('Snapshot registrado!', 'OK', { duration: 3000 }); },
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
