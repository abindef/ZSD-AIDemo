using Inno.CorePlatform.Finance.Domain.Common;
using Inno.CorePlatform.Finance.Domain.Events;
using Inno.CorePlatform.Finance.Domain.Exceptions;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

/// <summary>
/// 应收单聚合根
/// </summary>
public class Receivable : Entity<ReceivableId>, IAggregateRoot
{
    // 私有构造函数，防止直接实例化
    private Receivable() { }

    /// <summary>
    /// 金额
    /// </summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>
    /// 状态
    /// </summary>
    public ReceivableStatus Status { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public string CustomerId { get; private set; } = null!;

    /// <summary>
    /// 账期
    /// </summary>
    public PaymentTerm PaymentTerm { get; private set; } = null!;

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime DueDate { get; private set; }

    // 导航属性（只读集合）
    private readonly List<Claim> _claims = new();
    public IReadOnlyCollection<Claim> Claims => _claims.AsReadOnly();

    /// <summary>
    /// 工厂方法 - 创建应收单
    /// </summary>
    public static Receivable Create(
        ReceivableId id,
        Money amount,
        string customerId,
        PaymentTerm paymentTerm)
    {
        Guard.Against.Null(id, nameof(id));
        Guard.Against.Null(amount, nameof(amount));
        Guard.Against.NullOrWhiteSpace(customerId, nameof(customerId));
        Guard.Against.Null(paymentTerm, nameof(paymentTerm));

        var receivable = new Receivable
        {
            Id = id,
            Amount = amount,
            CustomerId = customerId,
            PaymentTerm = paymentTerm,
            Status = ReceivableStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            DueDate = paymentTerm.CalculateDueDate(DateTime.UtcNow)
        };

        // 发布领域事件
        receivable.AddDomainEvent(new ReceivableCreatedEvent(id, amount, customerId));

        return receivable;
    }

    /// <summary>
    /// 认款
    /// </summary>
    public void AddClaim(Money claimAmount, ClaimType type)
    {
        // 不变量校验
        if (Status == ReceivableStatus.Settled)
            throw new BusinessException("RECEIVABLE_SETTLED", "已核销的应收单不能再认款");

        if (Status == ReceivableStatus.Cancelled)
            throw new BusinessException("RECEIVABLE_CANCELLED", "已取消的应收单不能认款");

        Guard.Against.Null(claimAmount, nameof(claimAmount));

        // 业务逻辑
        var claim = Claim.Create(this.Id, claimAmount, type);
        _claims.Add(claim);

        // 状态流转
        var remaining = GetRemainingAmount();
        if (remaining.TaxIncluded == 0)
        {
            Status = ReceivableStatus.Settled;
            AddDomainEvent(new ReceivableSettledEvent(Id, Amount));
        }
        else if (_claims.Count > 0)
        {
            Status = ReceivableStatus.PartialClaimed;
        }
    }

    /// <summary>
    /// 取消应收单
    /// </summary>
    public void Cancel()
    {
        if (Status == ReceivableStatus.Settled)
            throw new BusinessException("RECEIVABLE_SETTLED", "已核销的应收单不能取消");

        if (_claims.Count > 0)
            throw new BusinessException("RECEIVABLE_HAS_CLAIMS", "已有认款记录的应收单不能取消");

        Status = ReceivableStatus.Cancelled;
    }

    /// <summary>
    /// 获取剩余金额
    /// </summary>
    public Money GetRemainingAmount()
    {
        if (_claims.Count == 0)
            return Amount;

        var totalClaimed = _claims.Aggregate(Money.Zero, (acc, c) => acc + c.Amount);
        return Amount - totalClaimed;
    }
}
