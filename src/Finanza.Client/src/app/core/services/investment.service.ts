import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Investment, InvestmentPortfolio } from '../models/investment.model';
import { environment } from '../../../environments/environment';

export interface InvestmentRequest {
  name: string;
  type: number;
  investedAmount: number;
  currentValue: number;
}

@Injectable({ providedIn: 'root' })
export class InvestmentService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/investments`;

  getPortfolio(): Observable<InvestmentPortfolio> {
    return this.http.get<InvestmentPortfolio>(`${this.base}/portfolio`);
  }

  create(request: InvestmentRequest): Observable<Investment> {
    return this.http.post<Investment>(`${this.base}/`, request);
  }

  update(id: string, request: InvestmentRequest): Observable<Investment> {
    return this.http.put<Investment>(`${this.base}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
