using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;
using Inno.CorePlatform.Finance.Domain.Repositories;

namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 更新批量付款单状态处理器
/// </summary>
public class UpdatePaymentAutoStatusHandler : ICommandHandler<UpdatePaymentAutoStatusCommand, Result>
{
    private readonly IPaymentAutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentAutoStatusHandler(
        IPaymentAutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePaymentAutoStatusCommand command, CancellationToken cancellationToken = default)
    {
        // 1. 获取聚合根
        var paymentAutoItemId = PaymentAutoItemId.From(command.Id);
        var paymentAutoItem = await _repository.GetByIdAsync(paymentAutoItemId, cancellationToken);

        if (paymentAutoItem is null)
            return Result.Fail("NOT_FOUND", "批量付款单不存在");

        // 2. 验证状态值
        if (!Enum.IsDefined(typeof(PaymentAutoStatus), command.Status))
            return Result.Fail("INVALID_STATUS", "无效的状态值");

        try
        {
            // 3. 更新状态
            var newStatus = (PaymentAutoStatus)command.Status;
            paymentAutoItem.UpdateStatus(
                newStatus,
                command.OARequestId,
                command.OALastApprover,
                command.OALastApprovalTime,
                command.OAApprovalComment,
                command.UpdatedBy);

            // 4. 持久化
            await _repository.UpdateAsync(paymentAutoItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
