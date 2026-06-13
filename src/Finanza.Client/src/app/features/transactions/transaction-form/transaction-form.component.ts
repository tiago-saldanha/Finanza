import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { switchMap, of, Subject } from 'rxjs';

import { CategoryService } from '../../../core/services/category.service';
import { TransactionService } from '../../../core/services/transaction.service';
import { FinancialAccountService } from '../../../core/services/financial-account.service';
import { PatrimonyService } from '../../../core/services/patrimony.service';
import { LoanService } from '../../../core/services/loan.service';
import { Transaction } from '../../../core/models/transaction.model';
import { Category } from '../../../core/models/category.model';
import { FinancialAccount } from '../../../core/models/financial-account.model';
import { Asset, Investment, Liability } from '../../../core/models/patrimony.model';
import { Loan } from '../../../core/models/loan.model';
import { CurrencyMaskDirective } from '../../../core/directives/currency-mask.directive';

export interface TransactionFormData {
  transaction?: Transaction;
}

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    CurrencyMaskDirective,
  ],
  templateUrl: './transaction-form.component.html',
  styleUrl: './transaction-form.component.scss',
})
export class TransactionFormComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly transactionService = inject(TransactionService);
  private readonly accountService = inject(FinancialAccountService);
  private readonly patrimonyService = inject(PatrimonyService);
  private readonly loanService = inject(LoanService);
  private readonly dialogRef = inject(MatDialogRef<TransactionFormComponent>);
  private readonly dialogData = inject<TransactionFormData>(MAT_DIALOG_DATA, { optional: true });
  private readonly destroy$ = new Subject<void>();

  categories  = signal<Category[]>([]);
  accounts    = signal<FinancialAccount[]>([]);
  assets      = signal<Asset[]>([]);
  liabilities = signal<Liability[]>([]);
  investments = signal<Investment[]>([]);
  loans       = signal<Loan[]>([]);
  saving      = signal(false);

  get isEdit(): boolean {
    return !!this.dialogData?.transaction;
  }

  form = this.fb.group({
    description:          ['', [Validators.required, Validators.minLength(2)]],
    amount:               [null as number | null, [Validators.required, Validators.min(0.01)]],
    transactionType:      ['', Validators.required],
    categoryId:           [null as string | null],
    accountId:            [null as string | null],
    destinationAccountId: [null as string | null],
    dueDate:              [null as Date | null, Validators.required],
    paymentDate:          [null as Date | null],
    assetId:              [null as string | null],
    liabilityId:          [null as string | null],
    loanReceivableId:     [null as string | null],
    investmentId:         [null as string | null],
  });

  isTransfer = computed(() => this.form.get('transactionType')?.value === 'Transfer');
  linkType   = signal<'none' | 'asset' | 'liability' | 'loan' | 'investment'>('none');

  ngOnInit(): void {
    this.categoryService.getAll().subscribe(cats => this.categories.set(cats));
    this.accountService.getAll().subscribe(accounts => this.accounts.set(accounts));
    this.patrimonyService.getNetWorth().subscribe(nw => {
      this.assets.set(nw.assets);
      this.liabilities.set(nw.liabilities);
      this.investments.set(nw.investments);
    });
    this.loanService.getAll().subscribe(loans => {
      this.loans.set(loans.filter(l => !l.isSettled));
      if (this.isEdit) this.fillForm(this.dialogData!.transaction!);
    });
  }

  setLinkType(type: 'none' | 'asset' | 'liability' | 'loan' | 'investment'): void {
    this.linkType.set(type);
    this.form.patchValue({ assetId: null, liabilityId: null, loanReceivableId: null, investmentId: null });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private fillForm(tx: Transaction): void {
    this.form.patchValue({
      description:          tx.description,
      amount:               tx.amount,
      transactionType:      tx.type,
      categoryId:           this.categories().find(c => c.name === tx.categoryName)?.id ?? null,
      accountId:            tx.accountId ?? null,
      destinationAccountId: tx.destinationAccountId ?? null,
      dueDate:              new Date(tx.dueDate),
      paymentDate:          tx.paymentDate ? new Date(tx.paymentDate) : null,
      assetId:              tx.assetId ?? null,
      liabilityId:          tx.liabilityId ?? null,
      loanReceivableId:     tx.loanReceivableId ?? null,
      investmentId:         tx.investmentId ?? null,
    });
    if (tx.assetId)               this.linkType.set('asset');
    else if (tx.liabilityId)      this.linkType.set('liability');
    else if (tx.loanReceivableId) this.linkType.set('loan');
    else if (tx.investmentId)     this.linkType.set('investment');
  }

  save(): void {
    if (this.form.invalid) return;

    this.saving.set(true);
    const val         = this.form.value;
    const dueDate     = val.dueDate as Date;
    const paymentDate = val.paymentDate as Date | null;
    const type        = val.transactionType as 'Revenue' | 'Expense' | 'Transfer';

    const save$ = this.isEdit
      ? this.transactionService.update(this.dialogData!.transaction!.id, {
          description:          val.description!,
          amount:               val.amount!,
          transactionType:      type,
          categoryId:           val.categoryId ?? undefined,
          accountId:            val.accountId ?? undefined,
          destinationAccountId: val.destinationAccountId ?? undefined,
          dueDate:              dueDate.toISOString(),
          assetId:              val.assetId ?? undefined,
          liabilityId:          val.liabilityId ?? undefined,
          loanReceivableId:     val.loanReceivableId ?? undefined,
          investmentId:         val.investmentId ?? undefined,
        })
      : this.transactionService.create({
          description:          val.description!,
          amount:               val.amount!,
          transactionType:      type,
          categoryId:           val.categoryId ?? undefined,
          accountId:            val.accountId ?? undefined,
          destinationAccountId: val.destinationAccountId ?? undefined,
          dueDate:              dueDate.toISOString(),
          createdAt:            new Date().toISOString(),
          assetId:              val.assetId ?? undefined,
          liabilityId:          val.liabilityId ?? undefined,
          loanReceivableId:     val.loanReceivableId ?? undefined,
          investmentId:         val.investmentId ?? undefined,
        });

    save$.pipe(
      switchMap(tx => {
        const alreadyPaid = this.isEdit && this.dialogData!.transaction!.status === 'Paid';
        if (paymentDate && !alreadyPaid) {
          return this.transactionService.pay(tx.id, { paymentDate: paymentDate.toISOString() });
        }
        return of(tx);
      }),
    ).subscribe({
      next: (tx) => {
        this.saving.set(false);
        this.dialogRef.close(tx);
      },
      error: () => this.saving.set(false),
    });
  }
}
