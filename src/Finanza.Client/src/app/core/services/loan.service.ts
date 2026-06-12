import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loan, LoanSummary } from '../models/loan.model';
import { environment } from '../../../environments/environment';

export interface LoanRequest {
  borrowerName: string;
  totalAmount: number;
  startDate: string;
  dueDate: string;
  notes?: string;
  installmentCount: number;
}

export interface PayInstallmentRequest {
  paidAt: string;
}

@Injectable({ providedIn: 'root' })
export class LoanService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/loans`;

  getAll(): Observable<Loan[]>                                          { return this.http.get<Loan[]>(`${this.base}/`); }
  getSummary(): Observable<LoanSummary>                                 { return this.http.get<LoanSummary>(`${this.base}/summary`); }
  getById(id: string): Observable<Loan>                                 { return this.http.get<Loan>(`${this.base}/${id}`); }
  create(r: LoanRequest): Observable<Loan>                             { return this.http.post<Loan>(`${this.base}/`, r); }
  update(id: string, r: LoanRequest): Observable<Loan>                 { return this.http.put<Loan>(`${this.base}/${id}`, r); }
  delete(id: string): Observable<void>                                  { return this.http.delete<void>(`${this.base}/${id}`); }
  payInstallment(installmentId: string, r: PayInstallmentRequest): Observable<unknown> {
    return this.http.post(`${this.base}/installments/${installmentId}/pay`, r);
  }
  unpayInstallment(installmentId: string): Observable<unknown> {
    return this.http.post(`${this.base}/installments/${installmentId}/unpay`, {});
  }
}
