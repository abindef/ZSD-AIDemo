using Inno.CorePlatform.Finance.Application.DTOs;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inno.CorePlatform.Finance.WebApi.Controllers;

/// <summary>
/// 应收单对外API控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ReceivablesController : ControllerBase
{
    private readonly IReceivableRepository _repository;
    private readonly ILogger<ReceivablesController> _logger;

    public ReceivablesController(
        IReceivableRepository repository,
        ILogger<ReceivablesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// 获取应收单
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ReceivableDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var receivableId = ReceivableId.From(id);
        var receivable = await _repository.GetByIdAsync(receivableId, cancellationToken);

        if (receivable is null)
            return NotFound(new ApiErrorResponse
            {
                Code = "RECEIVABLE_NOT_FOUND",
                Message = "应收单不存在"
            });

        var dto = MapToDto(receivable);
        return Ok(new ApiResponse<ReceivableDto> { Data = dto });
    }

    /// <summary>
    /// 根据客户ID获取应收单列表
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReceivableDto>>>> GetByCustomerId(
        string customerId,
        CancellationToken cancellationToken)
    {
        var receivables = await _repository.GetByCustomerIdAsync(customerId, cancellationToken);
        var dtos = receivables.Select(MapToDto).ToList();

        return Ok(new ApiResponse<IReadOnlyList<ReceivableDto>> { Data = dtos });
    }

    private static ReceivableDto MapToDto(Receivable receivable)
    {
        return new ReceivableDto
        {
            Id = receivable.Id.Value,
            TaxIncludedAmount = receivable.Amount.TaxIncluded,
            TaxExcludedAmount = receivable.Amount.TaxExcluded,
            TaxRate = receivable.Amount.TaxRate,
            CustomerId = receivable.CustomerId,
            Status = receivable.Status.ToString(),
            CreatedAt = receivable.CreatedAt,
            DueDate = receivable.DueDate,
            PaymentTermDays = receivable.PaymentTerm.Days,
            RemainingAmount = receivable.GetRemainingAmount().TaxIncluded,
            Claims = receivable.Claims.Select(c => new ClaimDto
            {
                Id = c.Id.Value,
                TaxIncludedAmount = c.Amount.TaxIncluded,
                Type = c.Type.ToString(),
                ClaimedAt = c.ClaimedAt,
                Remark = c.Remark
            }).ToList()
        };
    }
}

/// <summary>
/// API响应
/// </summary>
public record ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public T? Data { get; init; }
    public string? Message { get; init; }
}

/// <summary>
/// API错误响应
/// </summary>
public record ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public string Code { get; init; } = null!;
    public string Message { get; init; } = null!;
    public object? Details { get; init; }
}
