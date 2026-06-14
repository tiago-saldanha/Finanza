import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoanPayable, LoanPayableSummary } from '../models/loan-payable.model';
import { environment } from '../../../environments/environment';

export interface LoanPayableRequest {
  creditorName: string;
  totalAmount: number;
  startDate: string;
  dueDate: string;
  notes?: string;
  installmentCount: number;
}

export interface PayPayableInstallmentRequest {
  paidAt: string;
}

@Injectable({ providedIn: 'root' })
export class LoanPayableService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/loan-payables`;

  getAll(): Observable<LoanPayable[]>                                               { return this.http.get<LoanPayable[]>(`${this.base}/`); }
  getSummary(): Observable<LoanPayableSummary>                                      { return this.http.get<LoanPayableSummary>(`${this.base}/summary`); }
  getById(id: string): Observable<LoanPayable>                                      { return this.http.get<LoanPayable>(`${this.base}/${id}`); }
  create(r: LoanPayableRequest): Observable<LoanPayable>                           { return this.http.post<LoanPayable>(`${this.base}/`, r); }
  update(id: string, r: LoanPayableRequest): Observable<LoanPayable>               { return this.http.put<LoanPayable>(`${this.base}/${id}`, r); }
  delete(id: string): Observable<void>                                               { return this.http.delete<void>(`${this.base}/${id}`); }
  payInstallment(installmentId: string, r: PayPayableInstallmentRequest): Observable<unknown> {
    return this.http.post(`${this.base}/installments/${installmentId}/pay`, r);
  }
  unpayInstallment(installmentId: string): Observable<unknown> {
    return this.http.post(`${this.base}/installments/${installmentId}/unpay`, {});
  }
}
