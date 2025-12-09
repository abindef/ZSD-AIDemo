namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款明细ID值对象
/// </summary>
public record PaymentAutoDetailId
{
    public Guid Value { get; }

    private PaymentAutoDetailId(Guid value)
    {
        Value = value;
    }

    public static PaymentAutoDetailId Create() => new(Guid.NewGuid());
    public static PaymentAutoDetailId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
