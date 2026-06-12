import { Component, OnInit, OnDestroy, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { TransactionService } from '../../../core/services/transaction.service';
import { FilterStateService } from '../../../core/services/filter-state.service';
import {
  Transaction,
  TransactionStatus,
  TransactionType,
  TRANSACTION_STATUS_LABELS,
  TRANSACTION_TYPE_LABELS,
} from '../../../core/models/transaction.model';
import { combineLatest, debounceTime, distinctUntilChanged, Subject, switchMap, takeUntil, startWith } from 'rxjs';
import { TransactionFormComponent, TransactionFormData } from '../transaction-form/transaction-form.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { PayTransactionDialogComponent } from '../pay-transaction-dialog/pay-transaction-dialog.component';

interface TransactionFiltersState {
  search: string;
  status: string;
  type: string;
  period: string;
  startDate: string | null;
  endDate: string | null;
}

const PERIOD_OPTIONS = [
  { value: 'this_month',    label: 'Este Mês'        },
  { value: 'last_month',    label: 'Mês Anterior'    },
  { value: 'last_3_months', label: 'Últimos 3 Meses' },
  { value: 'this_year',     label: 'Este Ano'        },
  { value: 'custom',        label: 'Personalizado'   },
];

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatMenuModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatSortModule,
    MatPaginatorModule,
    MatDatepickerModule,
    ConfirmDialogComponent,
  ],
  templateUrl: './transaction-list.component.html',
  styleUrl: './transaction-list.component.scss',
})
export class TransactionListComponent implements OnInit, OnDestroy {
  private readonly transactionService = inject(TransactionService);
  private readonly filterState = inject(FilterStateService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroy$ = new Subject<void>();

  loading = signal(true);
  transactions = signal<Transaction[]>([]);

  private readonly FILTER_KEY = 'transactions';

  readonly periodOptions = PERIOD_OPTIONS;

  searchCtrl    = new FormControl('');
  statusCtrl    = new FormControl('');
  typeCtrl      = new FormControl('');
  periodCtrl    = new FormControl('this_month');
  startDateCtrl = new FormControl<Date | null>(this.firstDayOfMonth());
  endDateCtrl   = new FormControl<Date | null>(this.lastDayOfMonth());

  showCustomDates = computed(() => this.periodCtrl.value === 'custom');

  displayedColumns = ['type', 'description', 'amount', 'dueDate', 'paidAt', 'status', 'actions'];
  pageSize  = 10;
  pageIndex = signal(0);

  activeFilters = computed(() =>
    !!this.searchCtrl.value ||
    !!this.statusCtrl.value ||
    !!this.typeCtrl.value ||
    (this.periodCtrl.value !== 'this_month')
  );

  paginatedTransactions = computed(() => {
    const start = this.pageIndex() * this.pageSize;
    return this.transactions().slice(start, start + this.pageSize);
  });

  ngOnInit(): void {
    this.restoreFilters();

    this.periodCtrl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(period => {
      if (period && period !== 'custom') {
        const { start, end } = this.getPeriodDates(period);
        this.startDateCtrl.setValue(start, { emitEvent: false });
        this.endDateCtrl.setValue(end);
      }
    });

    combineLatest([
      this.searchCtrl.valueChanges.pipe(startWith(this.searchCtrl.value), debounceTime(400), distinctUntilChanged()),
      this.statusCtrl.valueChanges.pipe(startWith(this.statusCtrl.value)),
      this.typeCtrl.valueChanges.pipe(startWith(this.typeCtrl.value)),
      this.startDateCtrl.valueChanges.pipe(startWith(this.startDateCtrl.value)),
      this.endDateCtrl.valueChanges.pipe(startWith(this.endDateCtrl.value)),
    ])
      .pipe(
        switchMap(([search, status, type, startDate, endDate]) => {
          this.loading.set(true);
          this.pageIndex.set(0);
          this.persistFilters(search, status, type, startDate, endDate);
          return this.transactionService.search({
            search: search ?? '',
            status: (status as TransactionStatus) ?? '',
            type: (type as TransactionType) ?? '',
            startDate: startDate ?? null,
            endDate: endDate ? this.endOfDay(endDate) : null,
          });
        }),
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (list) => {
          this.transactions.set(list);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  private restoreFilters(): void {
    const saved = this.filterState.load<TransactionFiltersState>(this.FILTER_KEY);
    if (!saved) return;
    this.searchCtrl.setValue(saved.search ?? '', { emitEvent: false });
    this.statusCtrl.setValue(saved.status ?? '', { emitEvent: false });
    this.typeCtrl.setValue(saved.type ?? '', { emitEvent: false });
    this.periodCtrl.setValue(saved.period ?? 'this_month', { emitEvent: false });
    if (saved.period === 'custom') {
      this.startDateCtrl.setValue(saved.startDate ? new Date(saved.startDate) : this.firstDayOfMonth(), { emitEvent: false });
      this.endDateCtrl.setValue(saved.endDate ? new Date(saved.endDate) : this.lastDayOfMonth(), { emitEvent: false });
    } else {
      const { start, end } = this.getPeriodDates(saved.period ?? 'this_month');
      this.startDateCtrl.setValue(start, { emitEvent: false });
      this.endDateCtrl.setValue(end, { emitEvent: false });
    }
  }

  private persistFilters(search: string | null, status: string | null, type: string | null, startDate: Date | null, endDate: Date | null): void {
    this.filterState.save<TransactionFiltersState>(this.FILTER_KEY, {
      search:    search    ?? '',
      status:    status    ?? '',
      type:      type      ?? '',
      period:    this.periodCtrl.value ?? 'this_month',
      startDate: startDate ? startDate.toISOString() : null,
      endDate:   endDate   ? endDate.toISOString()   : null,
    });
  }

  private getPeriodDates(period: string): { start: Date; end: Date } {
    const now = new Date();
    const y = now.getFullYear();
    const m = now.getMonth();
    switch (period) {
      case 'last_month':    return { start: new Date(y, m - 1, 1), end: new Date(y, m, 0) };
      case 'last_3_months': return { start: new Date(y, m - 2, 1), end: new Date(y, m + 1, 0) };
      case 'this_year':     return { start: new Date(y, 0, 1),     end: new Date(y, 11, 31) };
      default:              return { start: new Date(y, m, 1),     end: new Date(y, m + 1, 0) };
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  reloadTransactions(): void {
    this.startDateCtrl.updateValueAndValidity({ emitEvent: true, onlySelf: false });
  }

  // ---- helpers ----

  private firstDayOfMonth(): Date {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), 1);
  }

  private lastDayOfMonth(): Date {
    const d = new Date();
    return new Date(d.getFullYear(), d.getMonth() + 1, 0);
  }

  /** Garante que o filtro de data final cobre o dia inteiro */
  private endOfDay(date: Date): Date {
    const d = new Date(date);
    d.setHours(23, 59, 59, 999);
    return d;
  }

  // ---- dialogs / actions ----

  openCreateDialog(): void {
    const ref = this.dialog.open(TransactionFormComponent, { width: '560px', disableClose: true });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Transação criada com sucesso!', 'OK', { duration: 3000 });
        this.reloadTransactions();
      }
    });
  }

  openEditDialog(tx: Transaction): void {
    const data: TransactionFormData = { transaction: tx };
    const ref = this.dialog.open(TransactionFormComponent, { width: '560px', disableClose: true, data });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Transação atualizada com sucesso!', 'OK', { duration: 3000 });
        this.reloadTransactions();
      }
    });
  }

  payTransaction(tx: Transaction): void {
    const ref = this.dialog.open(PayTransactionDialogComponent, {
      width: '420px',
      disableClose: true,
      data: { description: tx.description, amount: tx.amount },
    });
    ref.afterClosed().subscribe((paymentDate: Date | null) => {
      if (!paymentDate) return;
      this.transactionService.pay(tx.id, { paymentDate: paymentDate.toISOString() }).subscribe({
        next: () => {
          this.snackBar.open('Transação marcada como paga!', 'OK', { duration: 3000 });
          this.reloadTransactions();
        },
      });
    });
  }

  cancelTransaction(tx: Transaction): void {
    this.transactionService.cancel(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação cancelada.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  reopenTransaction(tx: Transaction): void {
    this.transactionService.reopen(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação reaberta.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  confirmRemove(tx: Transaction): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Excluir transação',
        message: `Tem certeza que deseja excluir "<strong>${tx.description}</strong>"? Esta ação não pode ser desfeita.`,
      },
    });
    ref.afterClosed().subscribe(confirmed => {
      if (confirmed) this.removeTransaction(tx);
    });
  }

  private removeTransaction(tx: Transaction): void {
    this.transactionService.remove(tx.id).subscribe({
      next: () => {
        this.snackBar.open('Transação excluída.', 'OK', { duration: 3000 });
        this.reloadTransactions();
      },
    });
  }

  clearFilters(): void {
    this.filterState.clear(this.FILTER_KEY);
    this.searchCtrl.setValue('');
    this.statusCtrl.setValue('');
    this.typeCtrl.setValue('');
    this.periodCtrl.setValue('this_month', { emitEvent: false });
    this.startDateCtrl.setValue(this.firstDayOfMonth(), { emitEvent: false });
    this.endDateCtrl.setValue(this.lastDayOfMonth());
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageIndex.set(event.pageIndex);
  }

  statusLabel(status: string): string {
    return TRANSACTION_STATUS_LABELS[status as TransactionStatus] ?? status;
  }

  typeLabel(type: string): string {
    return TRANSACTION_TYPE_LABELS[type as TransactionType] ?? type;
  }
}
