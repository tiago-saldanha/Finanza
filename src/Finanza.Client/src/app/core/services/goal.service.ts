import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Goal } from '../models/goal.model';
import { environment } from '../../../environments/environment';

export interface GoalRequest { name: string; targetAmount: number; currentAmount: number; targetDate: string; }
export interface ContributeRequest { amount: number; }

@Injectable({ providedIn: 'root' })
export class GoalService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/goals`;

  getAll(): Observable<Goal[]>                          { return this.http.get<Goal[]>(`${this.base}/`); }
  create(r: GoalRequest): Observable<Goal>              { return this.http.post<Goal>(`${this.base}/`, r); }
  update(id: string, r: GoalRequest): Observable<Goal>  { return this.http.put<Goal>(`${this.base}/${id}`, r); }
  contribute(id: string, r: ContributeRequest): Observable<Goal> { return this.http.post<Goal>(`${this.base}/${id}/contribute`, r); }
  delete(id: string): Observable<void>                  { return this.http.delete<void>(`${this.base}/${id}`); }
}
