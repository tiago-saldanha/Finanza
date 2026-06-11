import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { FinancialAccountService } from '../../../core/services/financial-account.service';
import { FinancialAccount, ACCOUNT_TYPE_OPTIONS } from '../../../core/models/financial-account.model';
import { CurrencyMaskDirective } from '../../../core/directives/currency-mask.directive';

export interface FinancialAccountFormData {
  account?: FinancialAccount;
}

@Component({
  selector: 'app-financial-account-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    CurrencyMaskDirective,
  ],
  templateUrl: './financial-account-form.component.html',
  styleUrl: './financial-account-form.component.scss',
})
export class FinancialAccountFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(FinancialAccountService);
  private readonly dialogRef = inject(MatDialogRef<FinancialAccountFormComponent>);
  private readonly data = inject<FinancialAccountFormData>(MAT_DIALOG_DATA, { optional: true });

  saving = signal(false);
  isEditMode = signal(false);
  readonly accountTypeOptions = ACCOUNT_TYPE_OPTIONS;

  form = this.fb.group({
    name:           ['', [Validators.required, Validators.minLength(2)]],
    type:           [null as number | null, Validators.required],
    initialBalance: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    if (this.data?.account) {
      this.isEditMode.set(true);
      const typeValue = ACCOUNT_TYPE_OPTIONS.find(o => o.label.includes(this.data!.account!.type))?.value
        ?? this.resolveTypeValue(this.data.account.type);
      this.form.patchValue({
        name:           this.data.account.name,
        type:           typeValue,
        initialBalance: this.data.account.initialBalance,
      });
    }
  }

  private resolveTypeValue(type: string): number {
    const map: Record<string, number> = {
      Checking: 0, Savings: 1, Wallet: 2, CreditCard: 3, Investment: 4,
    };
    return map[type] ?? 0;
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const val = this.form.value;
    const request = {
      name:           val.name!,
      type:           val.type!,
      initialBalance: val.initialBalance!,
    };

    const operation = this.isEditMode()
      ? this.service.update(this.data!.account!.id, request)
      : this.service.create(request);

    operation.subscribe({
      next: (account) => {
        this.saving.set(false);
        this.dialogRef.close(account);
      },
      error: () => this.saving.set(false),
    });
  }
}
