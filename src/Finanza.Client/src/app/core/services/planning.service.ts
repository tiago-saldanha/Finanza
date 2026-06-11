import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { FinancialPlanning } from '../models/planning.model';

@Injectable({ providedIn: 'root' })
export class PlanningService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/planning`;

  get(year?: number, month?: number): Observable<FinancialPlanning> {
    const params: Record<string, string> = {};
    if (year  != null) params['year']  = String(year);
    if (month != null) params['month'] = String(month);
    return this.http.get<FinancialPlanning>(`${this.baseUrl}/`, { params });
  }
}
