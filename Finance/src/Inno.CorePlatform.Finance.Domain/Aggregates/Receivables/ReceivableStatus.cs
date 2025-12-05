namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

/// <summary>
/// 应收单状态
/// </summary>
public enum ReceivableStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 部分认款
    /// </summary>
    PartialClaimed = 1,

    /// <summary>
    /// 已核销
    /// </summary>
    Settled = 2,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 3
}
