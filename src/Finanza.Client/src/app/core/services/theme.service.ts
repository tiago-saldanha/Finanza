import { Injectable, signal } from '@angular/core';

export interface Theme {
  id: string;
  label: string;
  primary: string;
  revenue: string;
  expense: string;
}

export const THEMES: Theme[] = [
  { id: 'theme-teal',    label: 'Teal Moderno', primary: '#00695c', revenue: '#1D9E75', expense: '#D85A30' },
  { id: 'theme-emerald', label: 'Esmeralda',    primary: '#065f46', revenue: '#059669', expense: '#dc2626' },
  { id: 'theme-blue',    label: 'Azul',         primary: '#1565c0', revenue: '#1976d2', expense: '#D85A30' },
  { id: 'theme-purple',  label: 'Roxo',         primary: '#6a1b9a', revenue: '#1D9E75', expense: '#D85A30' },
];

const THEME_KEY = 'fm_theme';
const DARK_KEY  = 'fm_dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly _current  = signal<Theme>(this.loadTheme());
  private readonly _darkMode = signal<boolean>(this.loadDark());

  readonly current  = this._current.asReadonly();
  readonly darkMode = this._darkMode.asReadonly();

  apply(theme: Theme): void {
    document.body.classList.remove(...THEMES.map(t => t.id));
    document.body.classList.add(theme.id);
    localStorage.setItem(THEME_KEY, theme.id);
    this._current.set(theme);
  }

  toggleDark(): void {
    const next = !this._darkMode();
    document.body.classList.toggle('dark', next);
    localStorage.setItem(DARK_KEY, next ? '1' : '0');
    this._darkMode.set(next);
  }

  private loadTheme(): Theme {
    const saved = localStorage.getItem(THEME_KEY);
    const theme = THEMES.find(t => t.id === saved) ?? THEMES[0];
    document.body.classList.add(theme.id);
    return theme;
  }

  private loadDark(): boolean {
    const saved = localStorage.getItem(DARK_KEY);
    // default: respeita preferência do sistema se não houver valor salvo
    const dark = saved !== null ? saved === '1' : window.matchMedia('(prefers-color-scheme: dark)').matches;
    document.body.classList.toggle('dark', dark);
    return dark;
  }
}
