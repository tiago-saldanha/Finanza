export interface Goal {
  id: string;
  name: string;
  targetAmount: number;
  currentAmount: number;
  remaining: number;
  progressRate: number;
  targetDate: string;
  isCompleted: boolean;
}
