import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { PatrimonyService } from '../../core/services/patrimony.service';
import {
  Asset, Liability, NetWorth,
  ASSET_TYPE_LABELS, LIABILITY_TYPE_LABELS,
  ASSET_TYPE_OPTIONS, LIABILITY_TYPE_OPTIONS,
} from '../../core/models/patrimony.model';
import {
  PatrimonyItemFormComponent,
  PatrimonyItemFormData,
} from './patrimony-item-form/patrimony-item-form.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-patrimony',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, MatCardModule, MatButtonModule, MatIconModule,
    MatDividerModule, MatProgressSpinnerModule, MatTooltipModule,
    MatDialogModule, MatSnackBarModule,
  ],
  templateUrl: './patrimony.component.html',
  styleUrl:    './patrimony.component.scss',
})
export class PatrimonyComponent implements OnInit {
  private readonly service  = inject(PatrimonyService);
  private readonly dialog   = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  loading  = signal(true);
  netWorth = signal<NetWorth | null>(null);

  readonly assetLabels     = ASSET_TYPE_LABELS;
  readonly liabilityLabels = LIABILITY_TYPE_LABELS;

  assetCoverage = computed(() => {
    const nw = this.netWorth();
    if (!nw || nw.totalAssets === 0) return 0;
    return Math.min((nw.netWorth / nw.totalAssets) * 100, 100);
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.service.getNetWorth().subscribe({
      next: (nw) => { this.netWorth.set(nw); this.loading.set(false); },
      error: ()  => this.loading.set(false),
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

  private confirmDelete(label: string, onConfirm: () => void): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Excluir', message: `Tem certeza que deseja excluir o ${label}?`, confirmLabel: 'Excluir' },
    });
    ref.afterClosed().subscribe(ok => { if (ok) onConfirm(); });
  }
}
