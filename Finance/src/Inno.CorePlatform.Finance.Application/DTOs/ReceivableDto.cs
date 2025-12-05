namespace Inno.CorePlatform.Finance.Application.DTOs;

/// <summary>
/// 应收单DTO
/// </summary>
public record ReceivableDto
{
    public Guid Id { get; init; }
    public decimal TaxIncludedAmount { get; init; }
    public decimal TaxExcludedAmount { get; init; }
    public int TaxRate { get; init; }
    public string CustomerId { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime DueDate { get; init; }
    public int PaymentTermDays { get; init; }
    public decimal RemainingAmount { get; init; }
    public IReadOnlyList<ClaimDto> Claims { get; init; } = Array.Empty<ClaimDto>();
}

/// <summary>
/// 认款记录DTO
/// </summary>
public record ClaimDto
{
    public Guid Id { get; init; }
    public decimal TaxIncludedAmount { get; init; }
    public string Type { get; init; } = null!;
    public DateTime ClaimedAt { get; init; }
    public string? Remark { get; init; }
}
