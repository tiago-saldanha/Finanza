using Finanza.Domain.Enums;
using Finanza.Infrastructure.Exceptions;
using Finanza.Infrastructure.Repositories;
using Finanza.Infrastructure.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Tests.Repositories.FinancialAccount
{
    public class FinancialAccountRepositoryTests(DatabaseFixture fixture)
        : IClassFixture<DatabaseFixture>
    {
        [Fact]
        public async Task AddAsync_ShouldAddAccount()
        {
            using var context = fixture.CreateContext();
            var sut = new FinancialAccountRepository(context);

            var account = Domain.Entities.Account.Create("Conta Corrente", AccountType.Checking, 1000);

            await sut.AddAsync(account);
            var entry = context.Entry(account);

            Assert.Equal(EntityState.Added, entry.State);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAccounts()
        {
            using var context = fixture.CreateContext();
            var sut = new FinancialAccountRepository(context);

            var a1 = Domain.Entities.Account.Create("Conta Corrente", AccountType.Checking, 1000);
            var a2 = Domain.Entities.Account.Create("Carteira",       AccountType.Wallet,   0);
            await context.Accounts.AddRangeAsync(a1, a2);
            await context.SaveChangesAsync();

            var result = await sut.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WhenAccountExists_ShouldReturnAccount()
        {
            using var context = fixture.CreateContext();
            var sut = new FinancialAccountRepository(context);

            var account = Domain.Entities.Account.Create("Poupança", AccountType.Savings, 500);
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();

            var result = await sut.GetByIdAsync(account.Id);

            Assert.Equal(account.Name.Value, result.Name.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WhenAccountDoesNotExist_ShouldThrow()
        {
            using var context = fixture.CreateContext();
            var sut = new FinancialAccountRepository(context);

            await Assert.ThrowsAsync<EntityNotFoundInfraException>(
                () => sut.GetByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAccount()
        {
            using var context = fixture.CreateContext();
            var sut = new FinancialAccountRepository(context);

            var account = Domain.Entities.Account.Create("Conta Corrente", AccountType.Checking, 1000);
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();

            await sut.DeleteAsync(account);
            var entry = context.Entry(account);

            Assert.Equal(EntityState.Deleted, entry.State);
        }
    }
}
