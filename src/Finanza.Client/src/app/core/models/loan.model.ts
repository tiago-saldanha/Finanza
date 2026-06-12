export interface LoanInstallment {
  id: string;
  number: number;
  amount: number;
  dueDate: string;
  paidAt?: string;
  isPaid: boolean;
  isOverdue: boolean;
}

export interface Loan {
  id: string;
  borrowerName: string;
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
  installments: LoanInstallment[];
}

export interface LoanSummary {
  totalLoaned: number;
  totalReceived: number;
  totalBalance: number;
  overdueCount: number;
}
