namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 更新批量付款单状态命令
/// </summary>
public record UpdatePaymentAutoStatusCommand
{
    public Guid Id { get; init; }
    public int Status { get; init; }
    public string? OARequestId { get; init; }
    public string? OALastApprover { get; init; }
    public DateTimeOffset? OALastApprovalTime { get; init; }
    public string? OAApprovalComment { get; init; }
    public Guid? UpdatedBy { get; init; }
}
