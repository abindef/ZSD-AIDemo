using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Domain.Services;

/// <summary>
/// 冲销领域服务
/// </summary>
public class WriteOffService
{
    /// <summary>
    /// 执行冲销操作
    /// </summary>
    /// <param name="receivable">应收单</param>
    /// <param name="amount">冲销金额</param>
    /// <param name="remark">备注</param>
    public void WriteOff(Receivable receivable, Money amount, string? remark = null)
    {
        receivable.AddClaim(amount, ClaimType.WriteOff);
    }
}
