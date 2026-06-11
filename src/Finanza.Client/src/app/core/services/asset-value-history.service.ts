import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AssetValueHistory } from '../models/asset-value-history.model';
import { environment } from '../../../environments/environment';

export interface UpdateAssetValueRequest {
  value: number;
  date: string;
}

@Injectable({ providedIn: 'root' })
export class AssetValueHistoryService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/assets`;

  getHistory(assetId: string): Observable<AssetValueHistory[]> {
    return this.http.get<AssetValueHistory[]>(`${this.base}/${assetId}/value-history`);
  }

  updateValue(assetId: string, request: UpdateAssetValueRequest): Observable<AssetValueHistory> {
    return this.http.post<AssetValueHistory>(`${this.base}/${assetId}/value`, request);
  }
}
