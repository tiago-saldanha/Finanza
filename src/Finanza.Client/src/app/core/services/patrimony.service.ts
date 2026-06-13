import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { Asset, AssetRequest, Liability, LiabilityInstallment, LiabilityRequest, NetWorth, PayLiabilityInstallmentRequest } from '../models/patrimony.model';

@Injectable({ providedIn: 'root' })
export class PatrimonyService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  getNetWorth(): Observable<NetWorth> {
    return this.http.get<NetWorth>(`${this.base}/net-worth/`);
  }

  // Assets
  createAsset(req: AssetRequest): Observable<Asset> {
    return this.http.post<Asset>(`${this.base}/assets/`, req);
  }
  updateAsset(id: string, req: AssetRequest): Observable<Asset> {
    return this.http.put<Asset>(`${this.base}/assets/${id}`, req);
  }
  deleteAsset(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/assets/${id}`);
  }

  // Liabilities
  createLiability(req: LiabilityRequest): Observable<Liability> {
    return this.http.post<Liability>(`${this.base}/liabilities/`, req);
  }
  updateLiability(id: string, req: LiabilityRequest): Observable<Liability> {
    return this.http.put<Liability>(`${this.base}/liabilities/${id}`, req);
  }
  deleteLiability(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/liabilities/${id}`);
  }
  payLiabilityInstallment(installmentId: string, req: PayLiabilityInstallmentRequest): Observable<LiabilityInstallment> {
    return this.http.post<LiabilityInstallment>(`${this.base}/liabilities/installments/${installmentId}/pay`, req);
  }
  unpayLiabilityInstallment(installmentId: string): Observable<LiabilityInstallment> {
    return this.http.post<LiabilityInstallment>(`${this.base}/liabilities/installments/${installmentId}/unpay`, {});
  }
}
