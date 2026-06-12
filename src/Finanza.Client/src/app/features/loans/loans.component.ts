import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';

import { LoanService, LoanRequest } from '../../core/services/loan.service';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';
import { Loan, LoanInstallment, LoanSummary } from '../../core/models/loan.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyMaskDirective } from '../../core/directives/currency-mask.directive';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DatePipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressBarModule,
    MatProgressSpinnerModule, MatSnackBarModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatChipsModule, MatExpansionModule,
    MatTooltipModule, MatDividerModule, CurrencyMaskDirective, DashboardTabsComponent,
  ],
  templateUrl: './loans.component.html',
  styleUrl:    './loans.component.scss',
})
export class LoansComponent implements OnInit {
  private readonly service  = inject(LoanService);
  private readonly dialog   = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb       = inject(FormBuilder);

  loading  = signal(true);
  saving   = signal(false);
  loans    = signal<Loan[]>([]);
  summary  = signal<LoanSummary | null>(null);
  editing  = signal<Loan | null>(null);
  showForm = signal(false);

  form = this.fb.group({
    borrowerName:     ['', Validators.required],
    totalAmount:      [null as number | null, [Validators.required, Validators.min(0.01)]],
    startDate:        ['', Validators.required],
    dueDate:          ['', Validators.required],
    notes:            [''],
    installmentCount: [1, [Validators.required, Validators.min(1), Validators.max(360)]],
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: loans => {
        this.loans.set(loans);
        this.service.getSummary().subscribe(s => this.summary.set(s));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openAdd(): void {
    this.editing.set(null);
    this.form.reset({ borrowerName: '', totalAmount: null, startDate: '', dueDate: '', notes: '', installmentCount: 1 });
    this.showForm.set(true);
  }

  openEdit(loan: Loan): void {
    this.editing.set(loan);
    this.form.setValue({
      borrowerName:     loan.borrowerName,
      totalAmount:      loan.totalAmount,
      startDate:        loan.startDate.substring(0, 10),
      dueDate:          loan.dueDate.substring(0, 10),
      notes:            loan.notes ?? '',
      installmentCount: loan.installmentCount,
    });
    this.showForm.set(true);
  }

  cancelForm(): void { this.showForm.set(false); }

  submit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const v = this.form.getRawValue();
    const req: LoanRequest = {
      borrowerName:     v.borrowerName!,
      totalAmount:      v.totalAmount!,
      startDate:        new Date(v.startDate!).toISOString(),
      dueDate:          new Date(v.dueDate!).toISOString(),
      notes:            v.notes || undefined,
      installmentCount: v.installmentCount!,
    };
    const edit = this.editing();
    const op = edit ? this.service.update(edit.id, req) : this.service.create(req);
    op.subscribe({
      next: () => {
        this.saving.set(false);
        this.showForm.set(false);
        this.snackBar.open(edit ? 'Empréstimo atualizado!' : 'Empréstimo criado!', 'OK', { duration: 3000 });
        this.load();
      },
      error: () => this.saving.set(false),
    });
  }

  payInstallment(installment: LoanInstallment): void {
    this.service.payInstallment(installment.id, { paidAt: new Date().toISOString() }).subscribe({
      next: () => { this.snackBar.open('Parcela marcada como paga!', 'OK', { duration: 2500 }); this.load(); },
    });
  }

  unpayInstallment(installment: LoanInstallment): void {
    this.service.unpayInstallment(installment.id).subscribe({
      next: () => { this.snackBar.open('Parcela reaberta.', 'OK', { duration: 2500 }); this.load(); },
    });
  }

  delete(loan: Loan): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Excluir empréstimo', message: `Excluir empréstimo de <strong>${loan.borrowerName}</strong>? Esta ação remove todas as parcelas.` },
    });
    ref.afterClosed().subscribe(ok => {
      if (ok) this.service.delete(loan.id).subscribe({ next: () => { this.snackBar.open('Excluído.', 'OK', { duration: 2500 }); this.load(); } });
    });
  }

  progressRate(loan: Loan): number {
    return loan.totalAmount > 0 ? Math.min((loan.totalPaid / loan.totalAmount) * 100, 100) : 0;
  }
}
