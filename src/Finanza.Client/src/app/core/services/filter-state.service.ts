import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FilterStateService {
  save<T>(key: string, value: T): void {
    try {
      localStorage.setItem(`finanza.filters.${key}`, JSON.stringify(value));
    } catch {}
  }

  load<T>(key: string): T | null {
    try {
      const raw = localStorage.getItem(`finanza.filters.${key}`);
      return raw ? (JSON.parse(raw) as T) : null;
    } catch {
      return null;
    }
  }

  clear(key: string): void {
    localStorage.removeItem(`finanza.filters.${key}`);
  }
}
