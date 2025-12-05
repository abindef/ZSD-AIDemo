using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Common;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Domain.Events;

/// <summary>
/// 应收单核销事件
/// </summary>
public record ReceivableSettledEvent : IDomainEvent
{
    public ReceivableId ReceivableId { get; }
    public Money TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public ReceivableSettledEvent(ReceivableId receivableId, Money totalAmount)
    {
        ReceivableId = receivableId;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}
