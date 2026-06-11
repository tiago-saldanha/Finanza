namespace Finanza.Application.DTOs.Responses
{
    public class FinancialPlanningResponse
    {
        // Mês de referência
        public int Year { get; init; }
        public int Month { get; init; }

        // Valores base do mês
        public decimal MonthlyRevenue { get; init; }
        public decimal MonthlyExpenses { get; init; }
        public decimal MonthlySavings { get; init; }

        // Taxa de Poupança = (Receita - Despesas) / Receita * 100
        public decimal SavingsRate { get; init; }

        // Regra 50-30-20: percentual real de despesas sobre a receita
        public decimal ExpenseRatio { get; init; }        // % da receita gasta
        public decimal SavingsRatio { get; init; }        // % da receita poupada (= SavingsRate)
        public decimal Rule50Target { get; init; } = 50;  // meta necessidades
        public decimal Rule30Target { get; init; } = 30;  // meta desejos
        public decimal Rule20Target { get; init; } = 20;  // meta poupança

        // Reserva de Emergência
        public decimal TotalAccountBalance { get; init; }       // soma dos saldos atuais das contas
        public decimal AvgMonthlyExpenses { get; init; }        // média dos últimos 3 meses
        public decimal EmergencyFundMonths { get; init; }       // meses cobertos pela reserva
        public decimal EmergencyFundTarget { get; init; } = 6;  // meta em meses (padrão: 6 meses)
    }
}
