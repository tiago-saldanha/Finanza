import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
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
import { MatTooltipModule } from '@angular/material/tooltip';

import { GoalService, GoalRequest } from '../../core/services/goal.service';
import { DashboardTabsComponent } from '../../shared/dashboard-tabs/dashboard-tabs.component';
import { Goal } from '../../core/models/goal.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyMaskDirective } from '../../core/directives/currency-mask.directive';

@Component({
  selector: 'app-goals',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DatePipe, DecimalPipe, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressBarModule,
    MatProgressSpinnerModule, MatSnackBarModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatTooltipModule, CurrencyMaskDirective, DashboardTabsComponent,
  ],
  templateUrl: './goals.component.html',
  styleUrl:    './goals.component.scss',
})
export class GoalsComponent implements OnInit {
  private readonly service  = inject(GoalService);
  private readonly dialog   = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly fb       = inject(FormBuilder);

  loading       = signal(true);
  saving        = signal(false);
  goals         = signal<Goal[]>([]);
  editing       = signal<Goal | null>(null);
  showForm      = signal(false);
  contributing  = signal<Goal | null>(null);

  form = this.fb.group({
    name:          ['', Validators.required],
    targetAmount:  [0,  [Validators.required, Validators.min(1)]],
    currentAmount: [0,  [Validators.required, Validators.min(0)]],
    targetDate:    ['', Validators.required],
  });

  contributeForm = this.fb.group({
    amount: [0, [Validators.required, Validators.min(0.01)]],
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: g => { this.goals.set(g); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openAdd(): void {
    this.editing.set(null);
    this.form.reset({ name: '', targetAmount: 0, currentAmount: 0, targetDate: '' });
    this.showForm.set(true);
  }

  openEdit(g: Goal): void {
    this.editing.set(g);
    this.form.setValue({
      name: g.name, targetAmount: g.targetAmount,
      currentAmount: g.currentAmount,
      targetDate: g.targetDate.substring(0, 10),
    });
    this.showForm.set(true);
  }

  cancelForm(): void { this.showForm.set(false); this.contributing.set(null); }

  submit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);
    const v = this.form.getRawValue();
    const req: GoalRequest = { name: v.name!, targetAmount: v.targetAmount!, currentAmount: v.currentAmount!, targetDate: v.targetDate! };
    const edit = this.editing();
    const op = edit ? this.service.update(edit.id, req) : this.service.create(req);
    op.subscribe({
      next: () => { this.saving.set(false); this.showForm.set(false); this.snackBar.open(edit ? 'Meta atualizada!' : 'Meta criada!', 'OK', { duration: 3000 }); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  openContribute(g: Goal): void {
    this.contributing.set(g);
    this.contributeForm.reset({ amount: 0 });
  }

  submitContribute(): void {
    const g = this.contributing();
    if (!g || this.contributeForm.invalid) return;
    this.saving.set(true);
    this.service.contribute(g.id, { amount: this.contributeForm.getRawValue().amount! }).subscribe({
      next: () => { this.saving.set(false); this.contributing.set(null); this.snackBar.open('Contribuição registrada!', 'OK', { duration: 3000 }); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  delete(g: Goal): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: { title: 'Excluir', message: `Excluir meta "${g.name}"?`, confirmLabel: 'Excluir' },
    });
    ref.afterClosed().subscribe(ok => { if (ok) this.service.delete(g.id).subscribe({ next: () => this.load() }); });
  }
}
