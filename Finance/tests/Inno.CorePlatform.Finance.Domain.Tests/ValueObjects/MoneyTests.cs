using FluentAssertions;
using Inno.CorePlatform.Finance.Domain.ValueObjects;
using Xunit;

namespace Inno.CorePlatform.Finance.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(1130, 13, 1000)]
    [InlineData(1060, 6, 1000)]
    [InlineData(1000, 0, 1000)]
    public void Create_WithValidParameters_ShouldCalculateTaxExcluded(
        decimal taxIncluded,
        int taxRate,
        decimal expectedTaxExcluded)
    {
        // Act
        var money = Money.Create(taxIncluded, taxRate);

        // Assert
        money.TaxIncluded.Should().Be(taxIncluded);
        money.TaxRate.Should().Be(taxRate);
        money.TaxExcluded.Should().BeApproximately(expectedTaxExcluded, 0.01m);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Act
        var act = () => Money.Create(-100m, 13);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WithInvalidTaxRate_ShouldThrowException()
    {
        // Act
        var act = () => Money.Create(1000m, 150);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Add_WithSameTaxRate_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Create(1000m, 13);
        var money2 = Money.Create(500m, 13);

        // Act
        var result = money1 + money2;

        // Assert
        result.TaxIncluded.Should().Be(1500m);
    }

    [Fact]
    public void Add_WithDifferentTaxRate_ShouldThrowException()
    {
        // Arrange
        var money1 = Money.Create(1000m, 13);
        var money2 = Money.Create(500m, 6);

        // Act
        var act = () => money1 + money2;

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Subtract_WithSameTaxRate_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Create(1000m, 13);
        var money2 = Money.Create(300m, 13);

        // Act
        var result = money1 - money2;

        // Assert
        result.TaxIncluded.Should().Be(700m);
    }

    [Fact]
    public void Zero_ShouldReturnZeroMoney()
    {
        // Act
        var zero = Money.Zero;

        // Assert
        zero.TaxIncluded.Should().Be(0);
        zero.TaxExcluded.Should().Be(0);
        zero.TaxRate.Should().Be(0);
    }
}
