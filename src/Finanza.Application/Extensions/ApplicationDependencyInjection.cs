using Finanza.Application.Interfaces.Services;
using Finanza.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.Application.Extensions
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITransactionAppService, TransactionAppService>();
            services.AddScoped<ICategoryAppService, CategoryAppService>();
            services.AddScoped<IFinancialAccountAppService, FinancialAccountAppService>();
            services.AddScoped<IFinancialPlanningAppService, FinancialPlanningAppService>();
            services.AddScoped<IAssetAppService, AssetAppService>();
            services.AddScoped<ILiabilityAppService, LiabilityAppService>();
            services.AddScoped<INetWorthAppService, NetWorthAppService>();
            services.AddScoped<IPatrimonySnapshotAppService, PatrimonySnapshotAppService>();
            services.AddScoped<IAssetValueHistoryAppService, AssetValueHistoryAppService>();
            services.AddScoped<IInvestmentAppService, InvestmentAppService>();
            services.AddScoped<IGoalAppService, GoalAppService>();
            services.AddScoped<IFireAppService, FireAppService>();
            services.AddScoped<ILoanAppService, LoanAppService>();

            return services;
        }
    }
}
