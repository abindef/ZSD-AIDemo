namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

/// <summary>
/// 应收单强类型ID
/// </summary>
public record ReceivableId
{
    public Guid Value { get; }

    private ReceivableId(Guid value)
    {
        Value = value;
    }

    public static ReceivableId Create()
    {
        return new ReceivableId(Guid.NewGuid());
    }

    public static ReceivableId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ReceivableId cannot be empty.", nameof(value));

        return new ReceivableId(value);
    }

    public override string ToString() => Value.ToString();
}
