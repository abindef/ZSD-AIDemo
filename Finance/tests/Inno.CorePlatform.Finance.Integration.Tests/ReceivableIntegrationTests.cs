using FluentAssertions;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.ValueObjects;
using Inno.CorePlatform.Finance.Infrastructure.Persistence;
using Inno.CorePlatform.Finance.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inno.CorePlatform.Finance.Integration.Tests;

public class ReceivableIntegrationTests : IDisposable
{
    private readonly FinanceDbContext _context;
    private readonly ReceivableRepository _repository;

    public ReceivableIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FinanceDbContext(options);
        _repository = new ReceivableRepository(_context);
    }

    [Fact]
    public async Task AddAndRetrieve_ShouldPersistReceivable()
    {
        // Arrange
        var receivable = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(1000m, 13),
            "CUST001",
            PaymentTerm.Net30);

        // Act
        await _repository.AddAsync(receivable);
        await _context.SaveChangesAsync();

        var retrieved = await _repository.GetByIdAsync(receivable.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(receivable.Id);
        retrieved.Amount.TaxIncluded.Should().Be(1000m);
        retrieved.CustomerId.Should().Be("CUST001");
    }

    [Fact]
    public async Task GetByCustomerId_ShouldReturnMatchingReceivables()
    {
        // Arrange
        var receivable1 = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(1000m, 13),
            "CUST001",
            PaymentTerm.Net30);

        var receivable2 = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(2000m, 13),
            "CUST001",
            PaymentTerm.Net60);

        var receivable3 = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(3000m, 13),
            "CUST002",
            PaymentTerm.Net30);

        await _repository.AddAsync(receivable1);
        await _repository.AddAsync(receivable2);
        await _repository.AddAsync(receivable3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync("CUST001");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.CustomerId.Should().Be("CUST001"));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
