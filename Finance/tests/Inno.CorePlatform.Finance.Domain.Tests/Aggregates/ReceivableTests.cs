using FluentAssertions;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Exceptions;
using Inno.CorePlatform.Finance.Domain.ValueObjects;
using Xunit;

namespace Inno.CorePlatform.Finance.Domain.Tests.Aggregates;

public class ReceivableTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateReceivable()
    {
        // Arrange
        var id = ReceivableId.Create();
        var amount = Money.Create(1000m, 13);
        var customerId = "CUST001";
        var paymentTerm = PaymentTerm.Net30;

        // Act
        var receivable = Receivable.Create(id, amount, customerId, paymentTerm);

        // Assert
        receivable.Should().NotBeNull();
        receivable.Id.Should().Be(id);
        receivable.Amount.Should().Be(amount);
        receivable.CustomerId.Should().Be(customerId);
        receivable.Status.Should().Be(ReceivableStatus.Pending);
        receivable.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void AddClaim_WithValidAmount_ShouldAddClaim()
    {
        // Arrange
        var receivable = CreateTestReceivable(1000m);
        var claimAmount = Money.Create(500m, 13);

        // Act
        receivable.AddClaim(claimAmount, ClaimType.BankTransfer);

        // Assert
        receivable.Claims.Should().HaveCount(1);
        receivable.Status.Should().Be(ReceivableStatus.PartialClaimed);
        receivable.GetRemainingAmount().TaxIncluded.Should().Be(500m);
    }

    [Fact]
    public void AddClaim_WithFullAmount_ShouldSettleReceivable()
    {
        // Arrange
        var receivable = CreateTestReceivable(1000m);
        var claimAmount = Money.Create(1000m, 13);

        // Act
        receivable.AddClaim(claimAmount, ClaimType.BankTransfer);

        // Assert
        receivable.Status.Should().Be(ReceivableStatus.Settled);
        receivable.GetRemainingAmount().TaxIncluded.Should().Be(0);
    }

    [Fact]
    public void AddClaim_WhenSettled_ShouldThrowBusinessException()
    {
        // Arrange
        var receivable = CreateTestReceivable(1000m);
        receivable.AddClaim(Money.Create(1000m, 13), ClaimType.BankTransfer);

        // Act
        var act = () => receivable.AddClaim(Money.Create(100m, 13), ClaimType.BankTransfer);

        // Assert
        act.Should().Throw<BusinessException>()
           .WithMessage("*已核销*");
    }

    [Fact]
    public void Cancel_WhenPending_ShouldCancelReceivable()
    {
        // Arrange
        var receivable = CreateTestReceivable(1000m);

        // Act
        receivable.Cancel();

        // Assert
        receivable.Status.Should().Be(ReceivableStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenHasClaims_ShouldThrowBusinessException()
    {
        // Arrange
        var receivable = CreateTestReceivable(1000m);
        receivable.AddClaim(Money.Create(500m, 13), ClaimType.BankTransfer);

        // Act
        var act = () => receivable.Cancel();

        // Assert
        act.Should().Throw<BusinessException>()
           .WithMessage("*认款记录*");
    }

    private static Receivable CreateTestReceivable(decimal amount)
    {
        return Receivable.Create(
            ReceivableId.Create(),
            Money.Create(amount, 13),
            "CUST001",
            PaymentTerm.Net30);
    }
}
