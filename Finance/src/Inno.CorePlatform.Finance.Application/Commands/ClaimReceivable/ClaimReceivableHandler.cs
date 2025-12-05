using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Inno.CorePlatform.Finance.Domain.ValueObjects;

namespace Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;

/// <summary>
/// 认款命令处理器
/// </summary>
public class ClaimReceivableHandler : ICommandHandler<ClaimReceivableCommand, Result>
{
    private readonly IReceivableRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ClaimReceivableHandler(
        IReceivableRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ClaimReceivableCommand command, CancellationToken cancellationToken = default)
    {
        // 1. 加载聚合
        var receivableId = ReceivableId.From(command.ReceivableId);
        var receivable = await _repository.GetByIdAsync(receivableId, cancellationToken);

        if (receivable is null)
            return Result.Fail("RECEIVABLE_NOT_FOUND", "应收单不存在");

        // 2. 解析认款类型
        if (!Enum.TryParse<ClaimType>(command.ClaimType, out var claimType))
            return Result.Fail("INVALID_CLAIM_TYPE", "无效的认款类型");

        // 3. 创建金额值对象
        var amount = Money.Create(command.Amount, command.TaxRate);

        // 4. 执行业务逻辑
        try
        {
            receivable.AddClaim(amount, claimType);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }

        // 5. 持久化
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
