export interface FinancialPlanning {
  year: number;
  month: number;
  monthlyRevenue: number;
  monthlyExpenses: number;
  monthlySavings: number;
  savingsRate: number;
  savingsRatio: number;
  expenseRatio: number;
  rule50Target: number;
  rule30Target: number;
  rule20Target: number;
  totalAccountBalance: number;
  avgMonthlyExpenses: number;
  emergencyFundMonths: number;
  emergencyFundTarget: number;
}
