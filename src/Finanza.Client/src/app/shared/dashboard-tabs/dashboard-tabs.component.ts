import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';

interface DashTab {
  path: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-dashboard-tabs',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatTabsModule, MatIconModule],
  templateUrl: './dashboard-tabs.component.html',
  styleUrl: './dashboard-tabs.component.scss',
})
export class DashboardTabsComponent {
  readonly tabs: DashTab[] = [
    { path: '/dashboard',           label: 'Visão Geral',      icon: 'dashboard' },
    { path: '/cash-flow',           label: 'Fluxo de Caixa',   icon: 'swap_vert' },
    { path: '/patrimony',           label: 'Patrimônio',       icon: 'account_balance' },
    { path: '/assets-liabilities',  label: 'Ativos x Passivos',icon: 'balance' },
    { path: '/investments',         label: 'Investimentos',    icon: 'show_chart' },
    { path: '/loans',               label: 'Empréstimos',      icon: 'handshake' },
    { path: '/goals',               label: 'Metas',            icon: 'flag' },
    { path: '/fire',                label: 'FIRE',             icon: 'local_fire_department' },
  ];
}
