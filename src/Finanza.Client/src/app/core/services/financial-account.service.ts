import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { FinancialAccount, FinancialAccountRequest } from '../models/financial-account.model';

@Injectable({ providedIn: 'root' })
export class FinancialAccountService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/financial-accounts`;

  getAll(): Observable<FinancialAccount[]> {
    return this.http.get<FinancialAccount[]>(`${this.baseUrl}/all`);
  }

  getById(id: string): Observable<FinancialAccount> {
    return this.http.get<FinancialAccount>(`${this.baseUrl}/${id}`);
  }

  create(request: FinancialAccountRequest): Observable<FinancialAccount> {
    return this.http.post<FinancialAccount>(`${this.baseUrl}/`, request);
  }

  update(id: string, request: FinancialAccountRequest): Observable<FinancialAccount> {
    return this.http.put<FinancialAccount>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
