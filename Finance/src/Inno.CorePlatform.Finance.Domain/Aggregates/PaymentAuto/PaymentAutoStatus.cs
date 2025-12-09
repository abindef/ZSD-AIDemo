namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款单状态枚举
/// </summary>
public enum PaymentAutoStatus
{
    /// <summary>
    /// 待提交
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 审批中
    /// </summary>
    Approving = 1,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 已驳回
    /// </summary>
    Rejected = 11
}
