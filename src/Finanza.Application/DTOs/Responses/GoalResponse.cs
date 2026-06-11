using Finanza.Domain.Entities;

namespace Finanza.Application.DTOs.Responses
{
    public class GoalResponse
    {
        public Guid     Id            { get; init; }
        public string   Name          { get; init; } = "";
        public decimal  TargetAmount  { get; init; }
        public decimal  CurrentAmount { get; init; }
        public decimal  Remaining     { get; init; }
        public decimal  ProgressRate  { get; init; }
        public DateTime TargetDate    { get; init; }
        public bool     IsCompleted   { get; init; }

        public static GoalResponse Create(Goal g) => new()
        {
            Id            = g.Id,
            Name          = g.Name.Value,
            TargetAmount  = g.TargetAmount.Value,
            CurrentAmount = g.CurrentAmount.Value,
            Remaining     = g.Remaining,
            ProgressRate  = g.ProgressRate,
            TargetDate    = g.TargetDate,
            IsCompleted   = g.IsCompleted,
        };
    }
}
