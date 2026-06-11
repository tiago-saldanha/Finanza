import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FireData } from '../models/fire.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FireService {
  private http = inject(HttpClient);

  get(): Observable<FireData> {
    return this.http.get<FireData>(`${environment.apiUrl}/fire/`);
  }
}
