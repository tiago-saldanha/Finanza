namespace Finanza.Application.DTOs.Requests
{
    public record GoalRequest(string Name, decimal TargetAmount, decimal CurrentAmount, DateTime TargetDate);
    public record ContributeGoalRequest(decimal Amount);
}
