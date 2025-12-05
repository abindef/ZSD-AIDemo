using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

namespace Inno.CorePlatform.Finance.Domain.Repositories;

/// <summary>
/// 应收单仓储接口
/// </summary>
public interface IReceivableRepository
{
    /// <summary>
    /// 根据ID获取应收单
    /// </summary>
    Task<Receivable?> GetByIdAsync(ReceivableId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据客户ID获取应收单列表
    /// </summary>
    Task<IReadOnlyList<Receivable>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加应收单
    /// </summary>
    Task AddAsync(Receivable receivable, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新应收单
    /// </summary>
    Task UpdateAsync(Receivable receivable, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除应收单
    /// </summary>
    Task DeleteAsync(Receivable receivable, CancellationToken cancellationToken = default);
}
