namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 删除批量付款单命令
/// </summary>
public record DeletePaymentAutoCommand(Guid Id, Guid? DeletedBy = null);
