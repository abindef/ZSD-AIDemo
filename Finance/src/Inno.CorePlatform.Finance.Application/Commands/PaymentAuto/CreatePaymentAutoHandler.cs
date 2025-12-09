using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;
using Inno.CorePlatform.Finance.Domain.Repositories;

namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 创建批量付款单处理器
/// </summary>
public class CreatePaymentAutoHandler : ICommandHandler<CreatePaymentAutoCommand, Result<Guid>>
{
    private readonly IPaymentAutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentAutoHandler(
        IPaymentAutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentAutoCommand command, CancellationToken cancellationToken = default)
    {
        // 1. 验证单号唯一性
        if (await _repository.ExistsByCodeAsync(command.Code, null, cancellationToken))
            return Result.Fail<Guid>("CODE_EXISTS", "单号已存在");

        // 2. 验证必填字段
        if (string.IsNullOrWhiteSpace(command.Code))
            return Result.Fail<Guid>("CODE_REQUIRED", "单号不能为空");

        if (string.IsNullOrWhiteSpace(command.CompanyName) || string.IsNullOrWhiteSpace(command.CompanyCode))
            return Result.Fail<Guid>("COMPANY_REQUIRED", "公司信息不能为空");

        if (command.Details == null || command.Details.Count == 0)
            return Result.Fail<Guid>("DETAILS_REQUIRED", "付款明细不能为空");

        // 3. 验证明细金额
        if (command.Details.Any(d => d.PaymentAmount <= 0))
            return Result.Fail<Guid>("INVALID_AMOUNT", "付款金额必须大于0");

        // 4. 创建聚合根
        var paymentAutoItem = PaymentAutoItem.Create(
            command.Code,
            command.BillDate,
            command.CompanyName,
            command.CompanyCode,
            command.CompanyId,
            command.Remark,
            command.OverallPostscript,
            command.DepartmentIdPath,
            command.DepartmentNamePath,
            command.CurrentDepartmentId,
            command.AttachmentIds,
            command.CreatedBy);

        // 5. 添加明细
        foreach (var detail in command.Details)
        {
            paymentAutoItem.AddDetail(
                detail.DebtDetailId,
                detail.PaymentAmount,
                detail.PaymentCode,
                detail.PaymentTime,
                detail.DiscountAmount,
                command.CreatedBy);
        }

        // 6. 添加供应商
        foreach (var agent in command.Agents)
        {
            var addedAgent = paymentAutoItem.AddAgent(
                agent.AgentId,
                agent.AgentName,
                agent.AccountName,
                agent.AccountNumber,
                agent.BankName,
                agent.BankCode,
                agent.PaymentType,
                agent.TransferRemark,
                command.CreatedBy);

            // 更新供应商的其他字段
            addedAgent.Update(
                agent.AgentId,
                agent.AgentName,
                agent.AccountName,
                agent.AccountNumber,
                agent.BankName,
                agent.BankCode,
                agent.PaymentType,
                agent.TransferRemark,
                agent.InvoiceNo,
                agent.ContractNo,
                agent.CrossBorderPayment,
                agent.TransactionCode,
                agent.TransactionRemark,
                agent.IsBondedGoods,
                agent.FeeBearer,
                agent.CustomsGoods,
                agent.AttachmentIds,
                agent.IsDebtTransfer,
                agent.DebtTransferAgentId,
                agent.DebtTransferAgentName,
                agent.DebtTransferAccount,
                agent.DebtTransferBankAccount,
                agent.DebtTransferBank,
                agent.DebtTransferBankName,
                agent.DebtTransferBankCode,
                agent.DebtTransferAttachment,
                agent.GroupPaymentId,
                command.CreatedBy);
        }

        // 7. 持久化
        await _repository.AddAsync(paymentAutoItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(paymentAutoItem.Id.Value);
    }
}
