import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, CurrencyPipe, DecimalPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { FireService } from '../../core/services/fire.service';
import { FireData } from '../../core/models/fire.model';

@Component({
  selector: 'app-fire',
  standalone: true,
  imports: [
    CommonModule, CurrencyPipe, DecimalPipe,
    MatCardModule, MatIconModule, MatProgressBarModule,
    MatProgressSpinnerModule, MatDividerModule, MatTooltipModule,
  ],
  templateUrl: './fire.component.html',
  styleUrl:    './fire.component.scss',
})
export class FireComponent implements OnInit {
  private readonly service = inject(FireService);

  loading = signal(true);
  data    = signal<FireData | null>(null);

  ngOnInit(): void {
    this.service.get().subscribe({
      next: d => { this.data.set(d); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  yearsLabel(years: number): string {
    if (years === -1) return 'Dados insuficientes';
    if (years === 0)  return '🎉 Independência atingida!';
    return `~${years} anos`;
  }
}
