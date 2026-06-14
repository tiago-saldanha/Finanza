export interface LoanPayableInstallment {
  id: string;
  number: number;
  amount: number;
  dueDate: string;
  paidAt?: string;
  isPaid: boolean;
  isOverdue: boolean;
}

export interface LoanPayable {
  id: string;
  creditorName: string;
  totalAmount: number;
  totalPaid: number;
  balance: number;
  startDate: string;
  dueDate: string;
  notes?: string;
  isSettled: boolean;
  hasOverdue: boolean;
  installmentCount: number;
  paidCount: number;
  installments: LoanPayableInstallment[];
}

export interface LoanPayableSummary {
  totalBorrowed: number;
  totalPaid: number;
  totalBalance: number;
  overdueCount: number;
}
