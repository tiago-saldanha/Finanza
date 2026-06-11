import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { startWith } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';

import { FinancialAccountService } from '../../../core/services/financial-account.service';
import { FinancialAccount, ACCOUNT_TYPE_LABELS } from '../../../core/models/financial-account.model';
import {
  FinancialAccountFormComponent,
  FinancialAccountFormData,
} from '../financial-account-form/financial-account-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-financial-account-list',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDividerModule,
  ],
  templateUrl: './financial-account-list.component.html',
  styleUrl: './financial-account-list.component.scss',
})
export class FinancialAccountListComponent implements OnInit {
  private readonly service = inject(FinancialAccountService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  loading = signal(true);
  accounts = signal<FinancialAccount[]>([]);
  searchCtrl = new FormControl('');

  readonly typeLabels = ACCOUNT_TYPE_LABELS;

  private searchValue = toSignal(
    this.searchCtrl.valueChanges.pipe(startWith('')),
    { initialValue: '' }
  );

  filteredAccounts = computed(() => {
    const search = (this.searchValue() ?? '').toLowerCase();
    return this.accounts().filter(a =>
      !search || a.name.toLowerCase().includes(search)
    );
  });

  totalBalance = computed(() =>
    this.accounts().reduce((sum, a) => sum + a.currentBalance, 0)
  );

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (accounts) => {
        this.accounts.set(accounts);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(FinancialAccountFormComponent, {
      width: '480px',
      disableClose: true,
    });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Conta criada com sucesso!', 'OK', { duration: 3000 });
        this.loadAll();
      }
    });
  }

  openEditDialog(account: FinancialAccount): void {
    const data: FinancialAccountFormData = { account };
    const ref = this.dialog.open(FinancialAccountFormComponent, {
      width: '480px',
      disableClose: true,
      data,
    });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Conta atualizada com sucesso!', 'OK', { duration: 3000 });
        this.loadAll();
      }
    });
  }

  openDeleteDialog(account: FinancialAccount): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Excluir Conta',
        message: `Tem certeza que deseja excluir a conta "${account.name}"? As transações vinculadas não serão excluídas.`,
        confirmLabel: 'Excluir',
        cancelLabel: 'Cancelar',
      },
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.service.delete(account.id).subscribe({
          next: () => {
            this.snackBar.open('Conta excluída!', 'OK', { duration: 3000 });
            this.loadAll();
          },
          error: () => this.snackBar.open('Erro ao excluir conta.', 'OK', { duration: 3000 }),
        });
      }
    });
  }

  typeLabel(account: FinancialAccount): string {
    return this.typeLabels[account.type] ?? account.type;
  }

  accountIcon(account: FinancialAccount): string {
    const map: Record<string, string> = {
      Checking:   'account_balance',
      Savings:    'savings',
      Wallet:     'account_balance_wallet',
      CreditCard: 'credit_card',
      Investment: 'trending_up',
    };
    return map[account.type] ?? 'account_balance';
  }
}
