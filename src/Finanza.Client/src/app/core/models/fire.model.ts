export interface FireData {
  annualExpenses: number;
  fireGoal: number;
  totalInvested: number;
  progressRate: number;
  monthlySavings: number;
  savingsRate: number;
  estimatedYearsToFire: number; // -1 = sem dados suficientes, 0 = já atingiu
}
