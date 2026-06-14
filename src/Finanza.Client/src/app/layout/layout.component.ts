import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import { ThemeService, THEMES, Theme } from '../core/services/theme.service';

interface NavItem {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
    MatMenuModule,
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss',
})
export class LayoutComponent {
  readonly auth = inject(AuthService);
  readonly themeService = inject(ThemeService);

  readonly themes = THEMES;
  today = new Date();
  isMobile = signal(window.innerWidth < 768);

  navItems: NavItem[] = [
    // ── Dashboards ──────────────────────────────────────
    { path: '/dashboard',           label: 'Visão Geral',       icon: 'dashboard' },
    { path: '/cash-flow',           label: 'Fluxo de Caixa',    icon: 'swap_vert' },
    { path: '/patrimony',           label: 'Patrimônio',        icon: 'account_balance' },
    { path: '/assets-liabilities',  label: 'Ativos x Passivos', icon: 'balance' },
    { path: '/investments',         label: 'Investimentos',     icon: 'show_chart' },
    { path: '/loans',               label: 'Empréstimos a rec.', icon: 'handshake' },
    { path: '/loan-payables',      label: 'Empréstimos a pag.', icon: 'credit_card' },
    { path: '/goals',               label: 'Metas',             icon: 'flag' },
    { path: '/fire',                label: 'FIRE',              icon: 'local_fire_department' },
    // ── Gestão ──────────────────────────────────────────
    { path: '/transactions',        label: 'Transações',        icon: 'receipt_long' },
    { path: '/categories',          label: 'Categorias',        icon: 'category' },
    { path: '/financial-accounts',  label: 'Contas',            icon: 'account_balance_wallet' },
    { path: '/planning',            label: 'Planejamento',      icon: 'insights' },
    { path: '/patrimony-snapshots', label: 'Histórico Patrim.', icon: 'timeline' },
    { path: '/account',             label: 'Minha Conta',       icon: 'manage_accounts' },
  ];

  applyTheme(theme: Theme): void {
    this.themeService.apply(theme);
  }
}
