namespace Inno.CorePlatform.Finance.Application.DTOs;

/// <summary>
/// 批量付款单DTO
/// </summary>
public record PaymentAutoItemDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public DateTime BillDate { get; init; }
    public Guid? CompanyId { get; init; }
    public string CompanyName { get; init; } = null!;
    public string CompanyCode { get; init; } = null!;
    public string? Remark { get; init; }
    public string? OverallPostscript { get; init; }
    public int Status { get; init; }
    public string StatusDescription { get; init; } = null!;
    public string? OARequestId { get; init; }
    public string? OALastApprover { get; init; }
    public DateTimeOffset? OALastApprovalTime { get; init; }
    public string? OAApprovalComment { get; init; }
    public string? DepartmentIdPath { get; init; }
    public string? DepartmentNamePath { get; init; }
    public string? CurrentDepartmentId { get; init; }
    public string? AttachmentIds { get; init; }
    public decimal TotalAmount { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<PaymentAutoDetailDto> Details { get; init; } = Array.Empty<PaymentAutoDetailDto>();
    public IReadOnlyList<PaymentAutoAgentDto> Agents { get; init; } = Array.Empty<PaymentAutoAgentDto>();
}

/// <summary>
/// 批量付款明细DTO
/// </summary>
public record PaymentAutoDetailDto
{
    public Guid Id { get; init; }
    public Guid PaymentAutoItemId { get; init; }
    public Guid DebtDetailId { get; init; }
    public string? PaymentCode { get; init; }
    public decimal PaymentAmount { get; init; }
    public DateTime? PaymentTime { get; init; }
    public decimal? DiscountAmount { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedAt { get; init; }
}

/// <summary>
/// 批量付款供应商DTO
/// </summary>
public record PaymentAutoAgentDto
{
    public Guid Id { get; init; }
    public Guid PaymentAutoItemId { get; init; }
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
    public Guid? CreatedBy { get; init; }
    public DateTime? CreatedAt { get; init; }
}

/// <summary>
/// 批量付款单列表项DTO（简化版）
/// </summary>
public record PaymentAutoListItemDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public DateTime BillDate { get; init; }
    public string CompanyName { get; init; } = null!;
    public decimal TotalAmount { get; init; }
    public int Status { get; init; }
    public string StatusDescription { get; init; } = null!;
    public string? Remark { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime? CreatedAt { get; init; }
}

/// <summary>
/// 状态统计DTO
/// </summary>
public record PaymentAutoStatusCountDto
{
    public int WaitSubmitCount { get; init; }
    public int HoldingCount { get; init; }
    public int FinishedCount { get; init; }
    public int RejectedCount { get; init; }
    public int AllCount { get; init; }
}

/// <summary>
/// 分页结果DTO
/// </summary>
public record PagedResultDto<T>
{
    public IReadOnlyList<T> List { get; init; } = Array.Empty<T>();
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
