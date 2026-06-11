import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { PlanningService } from '../../core/services/planning.service';
import { FinancialPlanning } from '../../core/models/planning.model';

@Component({
  selector: 'app-planning',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    DecimalPipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  templateUrl: './planning.component.html',
  styleUrl: './planning.component.scss',
})
export class PlanningComponent implements OnInit {
  private readonly service = inject(PlanningService);

  loading = signal(true);
  planning = signal<FinancialPlanning | null>(null);

  readonly now     = new Date();
  readonly months  = [
    { value: 1, label: 'Janeiro' }, { value: 2, label: 'Fevereiro' },
    { value: 3, label: 'Março' },   { value: 4, label: 'Abril' },
    { value: 5, label: 'Maio' },    { value: 6, label: 'Junho' },
    { value: 7, label: 'Julho' },   { value: 8, label: 'Agosto' },
    { value: 9, label: 'Setembro'},{ value: 10, label: 'Outubro' },
    { value: 11, label: 'Novembro'},{ value: 12, label: 'Dezembro'},
  ];
  readonly years = Array.from({ length: 5 }, (_, i) => this.now.getFullYear() - i);

  selectedYear  = new FormControl(this.now.getFullYear());
  selectedMonth = new FormControl(this.now.getMonth() + 1);

  // ── KPI helpers ──────────────────────────────────────────────────────────

  savingsRateColor = computed(() => {
    const rate = this.planning()?.savingsRate ?? 0;
    if (rate >= 20) return '#2e7d32'; // verde — dentro da meta 50-30-20
    if (rate >= 10) return '#f57c00'; // laranja
    return '#c62828';                  // vermelho
  });

  emergencyFundProgress = computed(() => {
    const p  = this.planning();
    if (!p) return 0;
    return Math.min((p.emergencyFundMonths / p.emergencyFundTarget) * 100, 100);
  });

  emergencyFundColor = computed(() => {
    const pct = this.emergencyFundProgress();
    if (pct >= 100) return '#2e7d32';
    if (pct >= 50)  return '#f57c00';
    return '#c62828';
  });

  expenseBarWidth = computed(() =>
    Math.min(this.planning()?.expenseRatio ?? 0, 100)
  );

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.get(this.selectedYear.value!, this.selectedMonth.value!).subscribe({
      next: (data) => {
        this.planning.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  monthLabel(m: number): string {
    return this.months.find(x => x.value === m)?.label ?? '';
  }
}
