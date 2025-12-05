using Inno.CorePlatform.Finance.Domain.Common;

namespace Inno.CorePlatform.Finance.Domain.ValueObjects;

/// <summary>
/// 金额值对象
/// </summary>
public record Money
{
    /// <summary>
    /// 含税金额（2位小数）
    /// </summary>
    public decimal TaxIncluded { get; }

    /// <summary>
    /// 不含税金额（10位小数）
    /// </summary>
    public decimal TaxExcluded { get; }

    /// <summary>
    /// 税率（整数百分比）
    /// </summary>
    public int TaxRate { get; }

    private Money(decimal taxIncluded, decimal taxExcluded, int taxRate)
    {
        TaxIncluded = taxIncluded;
        TaxExcluded = taxExcluded;
        TaxRate = taxRate;
    }

    public static Money Create(decimal taxIncluded, int taxRate)
    {
        Guard.Against.Negative(taxIncluded, nameof(taxIncluded));
        Guard.Against.OutOfRange(taxRate, nameof(taxRate), 0, 100);

        var taxExcluded = Math.Round(taxIncluded / (1 + taxRate / 100m), 10);
        return new Money(
            Math.Round(taxIncluded, 2),
            taxExcluded,
            taxRate
        );
    }

    public static Money Zero => new(0, 0, 0);

    public static Money operator +(Money a, Money b)
    {
        if (a.TaxRate != b.TaxRate)
            throw new InvalidOperationException("Cannot add money with different tax rates.");

        return new Money(
            a.TaxIncluded + b.TaxIncluded,
            a.TaxExcluded + b.TaxExcluded,
            a.TaxRate
        );
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.TaxRate != b.TaxRate)
            throw new InvalidOperationException("Cannot subtract money with different tax rates.");

        return new Money(
            a.TaxIncluded - b.TaxIncluded,
            a.TaxExcluded - b.TaxExcluded,
            a.TaxRate
        );
    }
}
