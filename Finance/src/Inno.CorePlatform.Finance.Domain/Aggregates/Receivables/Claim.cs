using Inno.CorePlatform.Finance.Domain.Common;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

/// <summary>
/// 认款记录
/// </summary>
public class Claim : Entity<ClaimId>
{
    private Claim() { }

    /// <summary>
    /// 关联的应收单ID
    /// </summary>
    public ReceivableId ReceivableId { get; private set; } = null!;

    /// <summary>
    /// 认款金额
    /// </summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>
    /// 认款类型
    /// </summary>
    public ClaimType Type { get; private set; }

    /// <summary>
    /// 认款时间
    /// </summary>
    public DateTime ClaimedAt { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 工厂方法
    /// </summary>
    public static Claim Create(ReceivableId receivableId, Money amount, ClaimType type, string? remark = null)
    {
        Guard.Against.Null(receivableId, nameof(receivableId));
        Guard.Against.Null(amount, nameof(amount));

        return new Claim
        {
            Id = ClaimId.Create(),
            ReceivableId = receivableId,
            Amount = amount,
            Type = type,
            ClaimedAt = DateTime.UtcNow,
            Remark = remark
        };
    }
}

/// <summary>
/// 认款ID
/// </summary>
public record ClaimId
{
    public Guid Value { get; }

    private ClaimId(Guid value)
    {
        Value = value;
    }

    public static ClaimId Create() => new(Guid.NewGuid());

    public static ClaimId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ClaimId cannot be empty.", nameof(value));

        return new ClaimId(value);
    }
}

/// <summary>
/// 认款类型
/// </summary>
public enum ClaimType
{
    /// <summary>
    /// 银行转账
    /// </summary>
    BankTransfer = 0,

    /// <summary>
    /// 现金
    /// </summary>
    Cash = 1,

    /// <summary>
    /// 支票
    /// </summary>
    Check = 2,

    /// <summary>
    /// 冲销
    /// </summary>
    WriteOff = 3
}
