namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款供应商ID值对象
/// </summary>
public record PaymentAutoAgentId
{
    public Guid Value { get; }

    private PaymentAutoAgentId(Guid value)
    {
        Value = value;
    }

    public static PaymentAutoAgentId Create() => new(Guid.NewGuid());
    public static PaymentAutoAgentId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
