using Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;
using Inno.CorePlatform.Finance.Application.Common;
using Inno.CorePlatform.Finance.Application.DTOs;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Inno.CorePlatform.Finance.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Inno.CorePlatform.Finance.Backend.Controllers;

/// <summary>
/// 应收单控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ReceivablesController : ControllerBase
{
    private readonly IReceivableRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReceivablesController> _logger;

    public ReceivablesController(
        IReceivableRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ReceivablesController> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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
    /// 创建应收单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReceivableDto>>> Create(
        [FromBody] CreateReceivableRequest request,
        CancellationToken cancellationToken)
    {
        var receivable = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(request.Amount, request.TaxRate),
            request.CustomerId,
            PaymentTerm.Create(request.PaymentTermDays));

        await _repository.AddAsync(receivable, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created receivable {ReceivableId} for customer {CustomerId}",
            receivable.Id, request.CustomerId);

        var dto = MapToDto(receivable);
        return CreatedAtAction(nameof(GetById), new { id = receivable.Id.Value }, new ApiResponse<ReceivableDto> { Data = dto });
    }

    /// <summary>
    /// 认款
    /// </summary>
    [HttpPost("{id:guid}/claims")]
    public async Task<ActionResult<ApiResponse<object>>> Claim(
        Guid id,
        [FromBody] ClaimReceivableCommand command,
        CancellationToken cancellationToken)
    {
        var handler = new ClaimReceivableHandler(_repository, _unitOfWork);
        var result = await handler.Handle(command with { ReceivableId = id }, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ApiErrorResponse
            {
                Code = result.Code ?? "CLAIM_FAILED",
                Message = result.Error ?? "认款失败"
            });

        return Ok(new ApiResponse<object> { Message = "认款成功" });
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
/// 创建应收单请求
/// </summary>
public record CreateReceivableRequest(
    decimal Amount,
    int TaxRate,
    string CustomerId,
    int PaymentTermDays
);

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
