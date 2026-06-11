export type AccountType = 'Checking' | 'Savings' | 'Wallet' | 'CreditCard' | 'Investment';

export interface FinancialAccount {
  id: string;
  name: string;
  type: AccountType;
  initialBalance: number;
  currentBalance: number;
}

export interface FinancialAccountRequest {
  name: string;
  type: number; // enum int value
  initialBalance: number;
}

export const ACCOUNT_TYPE_LABELS: Record<AccountType, string> = {
  Checking:   'Conta Corrente',
  Savings:    'Poupança',
  Wallet:     'Carteira',
  CreditCard: 'Cartão de Crédito',
  Investment: 'Investimento',
};

export const ACCOUNT_TYPE_OPTIONS: { value: number; label: string }[] = [
  { value: 0, label: 'Conta Corrente' },
  { value: 1, label: 'Poupança' },
  { value: 2, label: 'Carteira' },
  { value: 3, label: 'Cartão de Crédito' },
  { value: 4, label: 'Investimento' },
];
