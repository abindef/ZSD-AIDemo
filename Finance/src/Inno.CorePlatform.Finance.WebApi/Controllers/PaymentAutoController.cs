using Inno.CorePlatform.Finance.Application.Commands.PaymentAuto;
using Inno.CorePlatform.Finance.Application.DTOs;
using Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inno.CorePlatform.Finance.WebApi.Controllers;

/// <summary>
/// 批量付款单API控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PaymentAutoController : ControllerBase
{
    private readonly IPaymentAutoRepository _repository;
    private readonly CreatePaymentAutoHandler _createHandler;
    private readonly UpdatePaymentAutoHandler _updateHandler;
    private readonly DeletePaymentAutoHandler _deleteHandler;
    private readonly UpdatePaymentAutoStatusHandler _statusHandler;
    private readonly ILogger<PaymentAutoController> _logger;

    public PaymentAutoController(
        IPaymentAutoRepository repository,
        CreatePaymentAutoHandler createHandler,
        UpdatePaymentAutoHandler updateHandler,
        DeletePaymentAutoHandler deleteHandler,
        UpdatePaymentAutoStatusHandler statusHandler,
        ILogger<PaymentAutoController> logger)
    {
        _repository = repository;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _statusHandler = statusHandler;
        _logger = logger;
    }

    /// <summary>
    /// 获取批量付款单详情
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PaymentAutoItemDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var paymentAutoItemId = PaymentAutoItemId.From(id);
        var paymentAutoItem = await _repository.GetByIdAsync(paymentAutoItemId, cancellationToken);

        if (paymentAutoItem is null)
            return NotFound(new ApiErrorResponse
            {
                Code = "NOT_FOUND",
                Message = "批量付款单不存在"
            });

        var dto = MapToDto(paymentAutoItem);
        return Ok(new ApiResponse<PaymentAutoItemDto> { Data = dto });
    }

    /// <summary>
    /// 分页查询批量付款单
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<PaymentAutoListItemDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] int? status = null,
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var statusEnum = status.HasValue ? (PaymentAutoStatus?)status.Value : null;
        var (items, total) = await _repository.GetPagedAsync(
            page, limit, statusEnum, keyword, startDate, endDate, cancellationToken);

        var dtos = items.Select(MapToListItemDto).ToList();

        return Ok(new ApiResponse<PagedResultDto<PaymentAutoListItemDto>>
        {
            Data = new PagedResultDto<PaymentAutoListItemDto>
            {
                List = dtos,
                Total = total,
                Page = page,
                PageSize = limit
            }
        });
    }

    /// <summary>
    /// 获取状态统计
    /// </summary>
    [HttpGet("status-count")]
    public async Task<ActionResult<ApiResponse<PaymentAutoStatusCountDto>>> GetStatusCount(
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var statusCount = await _repository.GetStatusCountAsync(keyword, startDate, endDate, cancellationToken);

        return Ok(new ApiResponse<PaymentAutoStatusCountDto>
        {
            Data = new PaymentAutoStatusCountDto
            {
                WaitSubmitCount = statusCount.DraftCount,
                HoldingCount = statusCount.ApprovingCount,
                FinishedCount = statusCount.CompletedCount,
                RejectedCount = statusCount.RejectedCount,
                AllCount = statusCount.AllCount
            }
        });
    }

    /// <summary>
    /// 创建批量付款单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreatePaymentAutoCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ApiErrorResponse
            {
                Code = result.Code ?? "CREATE_FAILED",
                Message = result.Error ?? "创建失败"
            });

        return Ok(new ApiResponse<Guid> { Data = result.Value, Message = "创建成功" });
    }

    /// <summary>
    /// 更新批量付款单
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(
        Guid id,
        [FromBody] UpdatePaymentAutoCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new ApiErrorResponse
            {
                Code = "ID_MISMATCH",
                Message = "ID不匹配"
            });

        var result = await _updateHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ApiErrorResponse
            {
                Code = result.Code ?? "UPDATE_FAILED",
                Message = result.Error ?? "更新失败"
            });

        return Ok(new ApiResponse<object> { Message = "更新成功" });
    }

    /// <summary>
    /// 删除批量付款单
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePaymentAutoCommand(id);
        var result = await _deleteHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ApiErrorResponse
            {
                Code = result.Code ?? "DELETE_FAILED",
                Message = result.Error ?? "删除失败"
            });

        return Ok(new ApiResponse<object> { Message = "删除成功" });
    }

    /// <summary>
    /// 批量删除批量付款单
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBatch(
        [FromBody] Guid[] ids,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        foreach (var id in ids)
        {
            var command = new DeletePaymentAutoCommand(id);
            var result = await _deleteHandler.Handle(command, cancellationToken);

            if (!result.IsSuccess)
                errors.Add($"ID {id}: {result.Error}");
        }

        if (errors.Count > 0)
            return BadRequest(new ApiErrorResponse
            {
                Code = "PARTIAL_DELETE_FAILED",
                Message = "部分删除失败",
                Details = errors
            });

        return Ok(new ApiResponse<object> { Message = "批量删除成功" });
    }

    /// <summary>
    /// 更新批量付款单状态
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        Guid id,
        [FromBody] UpdatePaymentAutoStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new ApiErrorResponse
            {
                Code = "ID_MISMATCH",
                Message = "ID不匹配"
            });

        var result = await _statusHandler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ApiErrorResponse
            {
                Code = result.Code ?? "UPDATE_STATUS_FAILED",
                Message = result.Error ?? "状态更新失败"
            });

        return Ok(new ApiResponse<object> { Message = "状态更新成功" });
    }

    private static PaymentAutoItemDto MapToDto(PaymentAutoItem item)
    {
        return new PaymentAutoItemDto
        {
            Id = item.Id.Value,
            Code = item.Code,
            BillDate = item.BillDate,
            CompanyId = item.CompanyId,
            CompanyName = item.CompanyName,
            CompanyCode = item.CompanyCode,
            Remark = item.Remark,
            OverallPostscript = item.OverallPostscript,
            Status = (int)item.Status,
            StatusDescription = GetStatusDescription(item.Status),
            OARequestId = item.OARequestId,
            OALastApprover = item.OALastApprover,
            OALastApprovalTime = item.OALastApprovalTime,
            OAApprovalComment = item.OAApprovalComment,
            DepartmentIdPath = item.DepartmentIdPath,
            DepartmentNamePath = item.DepartmentNamePath,
            CurrentDepartmentId = item.CurrentDepartmentId,
            AttachmentIds = item.AttachmentIds,
            TotalAmount = item.CalculateTotalAmount(),
            CreatedBy = item.CreatedBy,
            CreatedAt = item.CreatedAt,
            UpdatedBy = item.UpdatedBy,
            UpdatedAt = item.UpdatedAt,
            Details = item.Details
                .Where(d => !d.IsDeleted)
                .Select(d => new PaymentAutoDetailDto
                {
                    Id = d.Id.Value,
                    PaymentAutoItemId = d.PaymentAutoItemId.Value,
                    DebtDetailId = d.DebtDetailId,
                    PaymentCode = d.PaymentCode,
                    PaymentAmount = d.PaymentAmount,
                    PaymentTime = d.PaymentTime,
                    DiscountAmount = d.DiscountAmount,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                }).ToList(),
            Agents = item.Agents
                .Where(a => !a.IsDeleted)
                .Select(a => new PaymentAutoAgentDto
                {
                    Id = a.Id.Value,
                    PaymentAutoItemId = a.PaymentAutoItemId.Value,
                    AgentId = a.AgentId,
                    AgentName = a.AgentName,
                    AccountName = a.AccountName,
                    AccountNumber = a.AccountNumber,
                    BankName = a.BankName,
                    BankCode = a.BankCode,
                    PaymentType = a.PaymentType,
                    TransferRemark = a.TransferRemark,
                    InvoiceNo = a.InvoiceNo,
                    ContractNo = a.ContractNo,
                    CrossBorderPayment = a.CrossBorderPayment,
                    TransactionCode = a.TransactionCode,
                    TransactionRemark = a.TransactionRemark,
                    IsBondedGoods = a.IsBondedGoods,
                    FeeBearer = a.FeeBearer,
                    CustomsGoods = a.CustomsGoods,
                    AttachmentIds = a.AttachmentIds,
                    IsDebtTransfer = a.IsDebtTransfer,
                    DebtTransferAgentId = a.DebtTransferAgentId,
                    DebtTransferAgentName = a.DebtTransferAgentName,
                    DebtTransferAccount = a.DebtTransferAccount,
                    DebtTransferBankAccount = a.DebtTransferBankAccount,
                    DebtTransferBank = a.DebtTransferBank,
                    DebtTransferBankName = a.DebtTransferBankName,
                    DebtTransferBankCode = a.DebtTransferBankCode,
                    DebtTransferAttachment = a.DebtTransferAttachment,
                    GroupPaymentId = a.GroupPaymentId,
                    CreatedBy = a.CreatedBy,
                    CreatedAt = a.CreatedAt
                }).ToList()
        };
    }

    private static PaymentAutoListItemDto MapToListItemDto(PaymentAutoItem item)
    {
        return new PaymentAutoListItemDto
        {
            Id = item.Id.Value,
            Code = item.Code,
            BillDate = item.BillDate,
            CompanyName = item.CompanyName,
            TotalAmount = item.CalculateTotalAmount(),
            Status = (int)item.Status,
            StatusDescription = GetStatusDescription(item.Status),
            Remark = item.Remark,
            CreatedAt = item.CreatedAt
        };
    }

    private static string GetStatusDescription(PaymentAutoStatus status)
    {
        return status switch
        {
            PaymentAutoStatus.Draft => "待提交",
            PaymentAutoStatus.Approving => "审批中",
            PaymentAutoStatus.Completed => "已完成",
            PaymentAutoStatus.Rejected => "已驳回",
            _ => "未知"
        };
    }
}
