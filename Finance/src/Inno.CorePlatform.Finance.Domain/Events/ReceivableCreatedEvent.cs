using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Common;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Domain.Events;

/// <summary>
/// 应收单创建事件
/// </summary>
public record ReceivableCreatedEvent : IDomainEvent
{
    public ReceivableId ReceivableId { get; }
    public Money Amount { get; }
    public string CustomerId { get; }
    public DateTime OccurredOn { get; }

    public ReceivableCreatedEvent(ReceivableId receivableId, Money amount, string customerId)
    {
        ReceivableId = receivableId;
        Amount = amount;
        CustomerId = customerId;
        OccurredOn = DateTime.UtcNow;
    }
}
