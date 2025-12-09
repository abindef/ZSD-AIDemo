namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款单ID值对象
/// </summary>
public record PaymentAutoItemId
{
    public Guid Value { get; }

    private PaymentAutoItemId(Guid value)
    {
        Value = value;
    }

    public static PaymentAutoItemId Create() => new(Guid.NewGuid());
    public static PaymentAutoItemId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
