namespace Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;

/// <summary>
/// 创建批量付款单命令
/// </summary>
public record CreatePaymentAutoCommand
{
    public string Code { get; init; } = null!;
    public DateTime BillDate { get; init; }
    public Guid? CompanyId { get; init; }
    public string CompanyName { get; init; } = null!;
    public string CompanyCode { get; init; } = null!;
    public string? Remark { get; init; }
    public string? OverallPostscript { get; init; }
    public string? DepartmentIdPath { get; init; }
    public string? DepartmentNamePath { get; init; }
    public string? CurrentDepartmentId { get; init; }
    public string? AttachmentIds { get; init; }
    public Guid? CreatedBy { get; init; }
    public IReadOnlyList<CreatePaymentAutoDetailCommand> Details { get; init; } = Array.Empty<CreatePaymentAutoDetailCommand>();
    public IReadOnlyList<CreatePaymentAutoAgentCommand> Agents { get; init; } = Array.Empty<CreatePaymentAutoAgentCommand>();
}

/// <summary>
/// 创建批量付款明细命令
/// </summary>
public record CreatePaymentAutoDetailCommand
{
    public Guid DebtDetailId { get; init; }
    public string? PaymentCode { get; init; }
    public decimal PaymentAmount { get; init; }
    public DateTime? PaymentTime { get; init; }
    public decimal? DiscountAmount { get; init; }
}

/// <summary>
/// 创建批量付款供应商命令
/// </summary>
public record CreatePaymentAutoAgentCommand
{
    public Guid? AgentId { get; init; }
    public string? AgentName { get; init; }
    public string? AccountName { get; init; }
    public string? AccountNumber { get; init; }
    public string? BankName { get; init; }
    public string? BankCode { get; init; }
    public string? PaymentType { get; init; }
    public string? TransferRemark { get; init; }
    public string? InvoiceNo { get; init; }
    public string? ContractNo { get; init; }
    public int? CrossBorderPayment { get; init; }
    public string? TransactionCode { get; init; }
    public string? TransactionRemark { get; init; }
    public int? IsBondedGoods { get; init; }
    public string? FeeBearer { get; init; }
    public string? CustomsGoods { get; init; }
    public string? AttachmentIds { get; init; }
    public bool? IsDebtTransfer { get; init; }
    public Guid? DebtTransferAgentId { get; init; }
    public string? DebtTransferAgentName { get; init; }
    public string? DebtTransferAccount { get; init; }
    public string? DebtTransferBankAccount { get; init; }
    public string? DebtTransferBank { get; init; }
    public string? DebtTransferBankName { get; init; }
    public string? DebtTransferBankCode { get; init; }
    public string? DebtTransferAttachment { get; init; }
    public int? GroupPaymentId { get; init; }
}
