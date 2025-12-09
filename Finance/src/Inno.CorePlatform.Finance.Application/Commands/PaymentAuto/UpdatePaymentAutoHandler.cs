using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;
using Inno.CorePlatform.Finance.Domain.Repositories;

namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 更新批量付款单处理器
/// </summary>
public class UpdatePaymentAutoHandler : ICommandHandler<UpdatePaymentAutoCommand, Result>
{
    private readonly IPaymentAutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentAutoHandler(
        IPaymentAutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdatePaymentAutoCommand command, CancellationToken cancellationToken = default)
    {
        // 1. 获取聚合根
        var paymentAutoItemId = PaymentAutoItemId.From(command.Id);
        var paymentAutoItem = await _repository.GetByIdAsync(paymentAutoItemId, cancellationToken);

        if (paymentAutoItem is null)
            return Result.Fail("NOT_FOUND", "批量付款单不存在");

        // 2. 验证单号唯一性
        if (await _repository.ExistsByCodeAsync(command.Code, command.Id, cancellationToken))
            return Result.Fail("CODE_EXISTS", "单号已存在");

        // 3. 验证必填字段
        if (string.IsNullOrWhiteSpace(command.Code))
            return Result.Fail("CODE_REQUIRED", "单号不能为空");

        if (string.IsNullOrWhiteSpace(command.CompanyName) || string.IsNullOrWhiteSpace(command.CompanyCode))
            return Result.Fail("COMPANY_REQUIRED", "公司信息不能为空");

        if (command.Details == null || command.Details.Count == 0)
            return Result.Fail("DETAILS_REQUIRED", "付款明细不能为空");

        // 4. 验证明细金额
        if (command.Details.Any(d => d.PaymentAmount <= 0))
            return Result.Fail("INVALID_AMOUNT", "付款金额必须大于0");

        try
        {
            // 5. 更新基本信息
            paymentAutoItem.Update(
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
                command.UpdatedBy);

            // 6. 处理明细（删除旧的，添加新的）
            var existingDetailIds = paymentAutoItem.Details
                .Where(d => !d.IsDeleted)
                .Select(d => d.Id.Value)
                .ToList();

            var newDetailIds = command.Details
                .Where(d => d.Id.HasValue)
                .Select(d => d.Id!.Value)
                .ToList();

            // 删除不在新列表中的明细
            foreach (var detailId in existingDetailIds.Where(id => !newDetailIds.Contains(id)))
            {
                paymentAutoItem.RemoveDetail(PaymentAutoDetailId.From(detailId));
            }

            // 添加或更新明细
            foreach (var detail in command.Details)
            {
                if (!detail.Id.HasValue)
                {
                    // 新增明细
                    paymentAutoItem.AddDetail(
                        detail.DebtDetailId,
                        detail.PaymentAmount,
                        detail.PaymentCode,
                        detail.PaymentTime,
                        detail.DiscountAmount,
                        command.UpdatedBy);
                }
                else
                {
                    // 更新现有明细
                    var existingDetail = paymentAutoItem.Details
                        .FirstOrDefault(d => d.Id.Value == detail.Id.Value && !d.IsDeleted);
                    existingDetail?.Update(
                        detail.PaymentAmount,
                        detail.PaymentCode,
                        detail.PaymentTime,
                        detail.DiscountAmount,
                        command.UpdatedBy);
                }
            }

            // 7. 处理供应商（删除旧的，添加新的）
            var existingAgentIds = paymentAutoItem.Agents
                .Where(a => !a.IsDeleted)
                .Select(a => a.Id.Value)
                .ToList();

            var newAgentIds = command.Agents
                .Where(a => a.Id.HasValue)
                .Select(a => a.Id!.Value)
                .ToList();

            // 删除不在新列表中的供应商
            foreach (var agentId in existingAgentIds.Where(id => !newAgentIds.Contains(id)))
            {
                paymentAutoItem.RemoveAgent(PaymentAutoAgentId.From(agentId));
            }

            // 添加或更新供应商
            foreach (var agent in command.Agents)
            {
                if (!agent.Id.HasValue)
                {
                    // 新增供应商
                    var addedAgent = paymentAutoItem.AddAgent(
                        agent.AgentId,
                        agent.AgentName,
                        agent.AccountName,
                        agent.AccountNumber,
                        agent.BankName,
                        agent.BankCode,
                        agent.PaymentType,
                        agent.TransferRemark,
                        command.UpdatedBy);

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
                        command.UpdatedBy);
                }
                else
                {
                    // 更新现有供应商
                    var existingAgent = paymentAutoItem.Agents
                        .FirstOrDefault(a => a.Id.Value == agent.Id.Value && !a.IsDeleted);
                    existingAgent?.Update(
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
                        command.UpdatedBy);
                }
            }

            // 8. 持久化
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
