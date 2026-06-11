export interface Investment {
  id: string;
  name: string;
  type: string;
  investedAmount: number;
  currentValue: number;
  return: number;
  returnRate: number;
}

export interface InvestmentTypeAllocation {
  type: string;
  value: number;
  percentage: number;
}

export interface InvestmentPortfolio {
  investments: Investment[];
  totalInvested: number;
  totalCurrent: number;
  totalReturn: number;
  totalReturnRate: number;
  allocations: InvestmentTypeAllocation[];
}

export const INVESTMENT_TYPE_OPTIONS = [
  { value: 0, label: 'Renda Fixa',  icon: 'savings'        },
  { value: 1, label: 'Ações',       icon: 'show_chart'     },
  { value: 2, label: 'FII',         icon: 'apartment'      },
  { value: 3, label: 'Cripto',      icon: 'currency_bitcoin'},
  { value: 4, label: 'Outros',      icon: 'category'       },
] as const;

export const INVESTMENT_TYPE_LABELS: Record<string, string> = {
  RendaFixa: 'Renda Fixa',
  Acoes:     'Ações',
  FII:       'FII',
  Cripto:    'Cripto',
  Other:     'Outros',
};
