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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { Observable } from 'rxjs';
import { PatrimonyService } from '../../../core/services/patrimony.service';
import {
  Asset, Liability,
  ASSET_TYPE_OPTIONS, LIABILITY_TYPE_OPTIONS,
} from '../../../core/models/patrimony.model';
import { CurrencyMaskDirective } from '../../../core/directives/currency-mask.directive';

export type PatrimonyItemKind = 'asset' | 'liability';

export interface PatrimonyItemFormData {
  kind:       PatrimonyItemKind;
  item?:      Asset | Liability;
}

@Component({
  selector: 'app-patrimony-item-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatDatepickerModule, MatNativeDateModule,
    CurrencyMaskDirective,
  ],
  templateUrl: './patrimony-item-form.component.html',
  styleUrl:    './patrimony-item-form.component.scss',
})
export class PatrimonyItemFormComponent implements OnInit {
  private readonly fb         = inject(FormBuilder);
  private readonly service    = inject(PatrimonyService);
  private readonly dialogRef  = inject(MatDialogRef<PatrimonyItemFormComponent>);
  readonly data               = inject<PatrimonyItemFormData>(MAT_DIALOG_DATA);

  saving     = signal(false);
  isEditMode = signal(false);

  get isAsset() { return this.data.kind === 'asset'; }
  get typeOptions() { return this.isAsset ? ASSET_TYPE_OPTIONS : LIABILITY_TYPE_OPTIONS; }
  get title() {
    const label = this.isAsset ? 'Ativo' : 'Passivo';
    return this.isEditMode() ? `Editar ${label}` : `Novo ${label}`;
  }

  form = this.fb.group({
    name:             ['', [Validators.required, Validators.minLength(2)]],
    type:             [null as number | null, Validators.required],
    value:            [null as number | null, [Validators.required, Validators.min(0)]],
    startDate:        [null as Date | null],
    dueDate:          [null as Date | null],
    notes:            [null as string | null],
    installmentCount: [null as number | null, [Validators.min(1), Validators.max(600)]],
  });

  ngOnInit(): void {
    if (this.data.item) {
      this.isEditMode.set(true);
      const item = this.data.item;
      const typeIdx = this.typeOptions.findIndex(o => o.label === this.resolveLabel(item.type));
      const patch: Record<string, unknown> = {
        name:  item.name,
        type:  typeIdx >= 0 ? this.typeOptions[typeIdx].value : 0,
        value: item.value,
      };
      if (!this.isAsset) {
        const li = item as Liability;
        patch['startDate'] = li.startDate ? new Date(li.startDate) : null;
        patch['dueDate']   = li.dueDate   ? new Date(li.dueDate)   : null;
        patch['notes']     = li.notes ?? null;
      }
      this.form.patchValue(patch);
    }
  }

  private resolveLabel(type: string): string {
    const opt = this.typeOptions.find(o =>
      o.label.toLowerCase().includes(type.toLowerCase()) ||
      type.toLowerCase().includes(o.label.toLowerCase().split(' ')[0])
    );
    return opt?.label ?? '';
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const val = this.form.value;

    const op$: Observable<Asset | Liability> = this.isAsset
      ? (this.isEditMode()
          ? this.service.updateAsset((this.data.item as Asset).id, { name: val.name!, type: val.type!, value: val.value! })
          : this.service.createAsset({ name: val.name!, type: val.type!, value: val.value! }))
      : (this.isEditMode()
          ? this.service.updateLiability((this.data.item as Liability).id, {
              name: val.name!, type: val.type!, value: val.value!,
              startDate: val.startDate ? val.startDate.toISOString() : undefined,
              dueDate:   val.dueDate   ? val.dueDate.toISOString()   : undefined,
              notes:     val.notes ?? undefined,
            })
          : this.service.createLiability({
              name: val.name!, type: val.type!, value: val.value!,
              startDate:        val.startDate ? val.startDate.toISOString() : undefined,
              dueDate:          val.dueDate   ? val.dueDate.toISOString()   : undefined,
              notes:            val.notes ?? undefined,
              installmentCount: val.installmentCount ?? 0,
            }));

    op$.subscribe({
      next: (result) => { this.saving.set(false); this.dialogRef.close(result); },
      error: ()      => this.saving.set(false),
    });
  }
}
