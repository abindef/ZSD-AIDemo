using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

namespace Inno.CorePlatform.Finance.Domain.Repositories;

/// <summary>
/// 批量付款单仓储接口
/// </summary>
public interface IPaymentAutoRepository
{
    /// <summary>
    /// 根据ID获取批量付款单（包含明细和供应商）
    /// </summary>
    Task<PaymentAutoItem?> GetByIdAsync(PaymentAutoItemId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据单号获取批量付款单
    /// </summary>
    Task<PaymentAutoItem?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询批量付款单
    /// </summary>
    Task<(IReadOnlyList<PaymentAutoItem> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        PaymentAutoStatus? status = null,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取状态统计
    /// </summary>
    Task<PaymentAutoStatusCount> GetStatusCountAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加批量付款单
    /// </summary>
    Task AddAsync(PaymentAutoItem paymentAutoItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新批量付款单
    /// </summary>
    Task UpdateAsync(PaymentAutoItem paymentAutoItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除批量付款单
    /// </summary>
    Task DeleteAsync(PaymentAutoItem paymentAutoItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查单号是否存在
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// 批量付款单状态统计
/// </summary>
public record PaymentAutoStatusCount
{
    /// <summary>
    /// 待提交数量
    /// </summary>
    public int DraftCount { get; init; }

    /// <summary>
    /// 审批中数量
    /// </summary>
    public int ApprovingCount { get; init; }

    /// <summary>
    /// 已完成数量
    /// </summary>
    public int CompletedCount { get; init; }

    /// <summary>
    /// 已驳回数量
    /// </summary>
    public int RejectedCount { get; init; }

    /// <summary>
    /// 全部数量
    /// </summary>
    public int AllCount => DraftCount + ApprovingCount + CompletedCount + RejectedCount;
}
