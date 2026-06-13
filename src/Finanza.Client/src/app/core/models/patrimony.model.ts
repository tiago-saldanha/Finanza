export type AssetType = 'BankAccount' | 'Vehicle' | 'Property' | 'Investment' | 'Other';
export type LiabilityType = 'Financing' | 'Loan' | 'CreditCard' | 'Other';

export interface Asset {
  id: string;
  name: string;
  type: AssetType;
  value: number;
}

export interface LiabilityInstallment {
  id: string;
  number: number;
  amount: number;
  dueDate: string;
  paidAt?: string;
  isPaid: boolean;
  isOverdue: boolean;
}

export interface Liability {
  id: string;
  name: string;
  type: LiabilityType;
  value: number;
  startDate?: string;
  dueDate?: string;
  notes?: string;
  totalPaid: number;
  balance: number;
  isSettled: boolean;
  hasOverdue: boolean;
  installmentCount: number;
  paidCount: number;
  installments: LiabilityInstallment[];
}

export interface NetWorth {
  totalAssets: number;
  totalInvestments: number;
  totalLiabilities: number;
  netWorth: number;
  assets: Asset[];
  liabilities: Liability[];
  investments: Investment[];
}

export interface Investment {
  id: string;
  name: string;
  type: string;
  investedAmount: number;
  currentValue: number;
  return: number;
  returnRate: number;
}

export interface AssetRequest {
  name: string;
  type: number;
  value: number;
}

export interface LiabilityRequest {
  name: string;
  type: number;
  value: number;
  startDate?: string;
  dueDate?: string;
  notes?: string;
  installmentCount?: number;
}

export interface PayLiabilityInstallmentRequest {
  paidAt: string;
}

export const ASSET_TYPE_OPTIONS: { value: number; label: string; icon: string }[] = [
  { value: 0, label: 'Conta Bancária',  icon: 'account_balance' },
  { value: 1, label: 'Veículo',         icon: 'directions_car' },
  { value: 2, label: 'Imóvel',          icon: 'home' },
  { value: 3, label: 'Investimento',    icon: 'trending_up' },
  { value: 4, label: 'Outro',           icon: 'category' },
];

export const LIABILITY_TYPE_OPTIONS: { value: number; label: string; icon: string }[] = [
  { value: 0, label: 'Financiamento',   icon: 'real_estate_agent' },
  { value: 1, label: 'Empréstimo',      icon: 'request_quote' },
  { value: 2, label: 'Cartão de Crédito', icon: 'credit_card' },
  { value: 3, label: 'Outro',           icon: 'category' },
];

export const ASSET_TYPE_LABELS: Record<string, string> = {
  BankAccount: 'Conta Bancária', Vehicle: 'Veículo', Property: 'Imóvel',
  Investment: 'Investimento', Other: 'Outro',
};

export const LIABILITY_TYPE_LABELS: Record<string, string> = {
  Financing: 'Financiamento', Loan: 'Empréstimo', CreditCard: 'Cartão de Crédito', Other: 'Outro',
};
