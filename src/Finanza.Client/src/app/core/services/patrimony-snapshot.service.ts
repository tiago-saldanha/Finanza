import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PatrimonySnapshot } from '../models/patrimony-snapshot.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PatrimonySnapshotService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/patrimony-snapshots`;

  getAll(): Observable<PatrimonySnapshot[]> {
    return this.http.get<PatrimonySnapshot[]>(`${this.base}/`);
  }

  create(): Observable<PatrimonySnapshot> {
    return this.http.post<PatrimonySnapshot>(`${this.base}/`, null);
  }
}
