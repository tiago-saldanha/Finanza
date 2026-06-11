namespace Finanza.Application.DTOs.Responses
{
    public class FireResponse
    {
        public decimal AnnualExpenses      { get; init; }
        public decimal FireGoal            { get; init; }  // AnnualExpenses × 25 (regra dos 4%)
        public decimal TotalInvested       { get; init; }
        public decimal ProgressRate        { get; init; }  // TotalInvested / FireGoal × 100
        public decimal MonthlySavings      { get; init; }
        public decimal SavingsRate         { get; init; }  // %
        public decimal EstimatedYearsToFire { get; init; } // -1 = sem dados suficientes
    }
}
