using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;
using Inno.CorePlatform.Finance.Domain.Repositories;

namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 删除批量付款单处理器
/// </summary>
public class DeletePaymentAutoHandler : ICommandHandler<DeletePaymentAutoCommand, Result>
{
    private readonly IPaymentAutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaymentAutoHandler(
        IPaymentAutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePaymentAutoCommand command, CancellationToken cancellationToken = default)
    {
        // 1. 获取聚合根
        var paymentAutoItemId = PaymentAutoItemId.From(command.Id);
        var paymentAutoItem = await _repository.GetByIdAsync(paymentAutoItemId, cancellationToken);

        if (paymentAutoItem is null)
            return Result.Fail("NOT_FOUND", "批量付款单不存在");

        try
        {
            // 2. 执行删除
            paymentAutoItem.Delete();

            // 3. 持久化
            await _repository.DeleteAsync(paymentAutoItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
