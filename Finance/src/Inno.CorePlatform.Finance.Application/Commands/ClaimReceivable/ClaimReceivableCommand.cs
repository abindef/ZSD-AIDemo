namespace Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;

/// <summary>
/// 认款命令
/// </summary>
public record ClaimReceivableCommand(
    Guid ReceivableId,
    decimal Amount,
    int TaxRate,
    string ClaimType,
    string? Remark = null
);
