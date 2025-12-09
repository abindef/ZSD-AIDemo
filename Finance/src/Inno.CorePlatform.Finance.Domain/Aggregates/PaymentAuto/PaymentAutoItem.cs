using Inno.CorePlatform.Finance.Domain.Common;
using Inno.CorePlatform.Finance.Domain.Exceptions;

namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款单聚合根
/// </summary>
public class PaymentAutoItem : Entity<PaymentAutoItemId>, IAggregateRoot
{
    private PaymentAutoItem() { }

    /// <summary>
    /// 单号
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// 单据日期
    /// </summary>
    public DateTime BillDate { get; private set; }

    /// <summary>
    /// 公司ID
    /// </summary>
    public Guid? CompanyId { get; private set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; private set; } = null!;

    /// <summary>
    /// 公司Code
    /// </summary>
    public string CompanyCode { get; private set; } = null!;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 整个附言
    /// </summary>
    public string? OverallPostscript { get; private set; }

    /// <summary>
    /// 状态
    /// </summary>
    public PaymentAutoStatus Status { get; private set; }

    /// <summary>
    /// OA请求Id
    /// </summary>
    public string? OARequestId { get; private set; }

    /// <summary>
    /// OA最后审批人
    /// </summary>
    public string? OALastApprover { get; private set; }

    /// <summary>
    /// OA最后审批时间
    /// </summary>
    public DateTimeOffset? OALastApprovalTime { get; private set; }

    /// <summary>
    /// OA审批意见
    /// </summary>
    public string? OAApprovalComment { get; private set; }

    /// <summary>
    /// 核算部门Id路径
    /// </summary>
    public string? DepartmentIdPath { get; private set; }

    /// <summary>
    /// 核算部门名称路径
    /// </summary>
    public string? DepartmentNamePath { get; private set; }

    /// <summary>
    /// 核算部门当前Id
    /// </summary>
    public string? CurrentDepartmentId { get; private set; }

    /// <summary>
    /// 附件Ids
    /// </summary>
    public string? AttachmentIds { get; private set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public Guid? CreatedBy { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; private set; }

    /// <summary>
    /// 更新人
    /// </summary>
    public Guid? UpdatedBy { get; private set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; private set; }

    // 导航属性（只读集合）
    private readonly List<PaymentAutoDetail> _details = new();
    public IReadOnlyCollection<PaymentAutoDetail> Details => _details.AsReadOnly();

    private readonly List<PaymentAutoAgent> _agents = new();
    public IReadOnlyCollection<PaymentAutoAgent> Agents => _agents.AsReadOnly();

    /// <summary>
    /// 工厂方法 - 创建批量付款单
    /// </summary>
    public static PaymentAutoItem Create(
        string code,
        DateTime billDate,
        string companyName,
        string companyCode,
        Guid? companyId = null,
        string? remark = null,
        string? overallPostscript = null,
        string? departmentIdPath = null,
        string? departmentNamePath = null,
        string? currentDepartmentId = null,
        string? attachmentIds = null,
        Guid? createdBy = null)
    {
        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(companyName, nameof(companyName));
        Guard.Against.NullOrWhiteSpace(companyCode, nameof(companyCode));

        return new PaymentAutoItem
        {
            Id = PaymentAutoItemId.Create(),
            Code = code,
            BillDate = billDate,
            CompanyId = companyId,
            CompanyName = companyName,
            CompanyCode = companyCode,
            Remark = remark,
            OverallPostscript = overallPostscript,
            Status = PaymentAutoStatus.Draft,
            DepartmentIdPath = departmentIdPath,
            DepartmentNamePath = departmentNamePath,
            CurrentDepartmentId = currentDepartmentId,
            AttachmentIds = attachmentIds,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    /// <summary>
    /// 更新批量付款单基本信息
    /// </summary>
    public void Update(
        string code,
        DateTime billDate,
        string companyName,
        string companyCode,
        Guid? companyId = null,
        string? remark = null,
        string? overallPostscript = null,
        string? departmentIdPath = null,
        string? departmentNamePath = null,
        string? currentDepartmentId = null,
        string? attachmentIds = null,
        Guid? updatedBy = null)
    {
        if (Status != PaymentAutoStatus.Draft && Status != PaymentAutoStatus.Rejected)
            throw new BusinessException("PAYMENT_CANNOT_UPDATE", "只有待提交或已驳回状态的付款单才能修改");

        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(companyName, nameof(companyName));
        Guard.Against.NullOrWhiteSpace(companyCode, nameof(companyCode));

        Code = code;
        BillDate = billDate;
        CompanyId = companyId;
        CompanyName = companyName;
        CompanyCode = companyCode;
        Remark = remark;
        OverallPostscript = overallPostscript;
        DepartmentIdPath = departmentIdPath;
        DepartmentNamePath = departmentNamePath;
        CurrentDepartmentId = currentDepartmentId;
        AttachmentIds = attachmentIds;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public void UpdateStatus(
        PaymentAutoStatus newStatus,
        string? oaRequestId = null,
        string? oaLastApprover = null,
        DateTimeOffset? oaLastApprovalTime = null,
        string? oaApprovalComment = null,
        Guid? updatedBy = null)
    {
        Status = newStatus;
        OARequestId = oaRequestId ?? OARequestId;
        OALastApprover = oaLastApprover ?? OALastApprover;
        OALastApprovalTime = oaLastApprovalTime ?? OALastApprovalTime;
        OAApprovalComment = oaApprovalComment ?? OAApprovalComment;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 添加付款明细
    /// </summary>
    public PaymentAutoDetail AddDetail(
        Guid debtDetailId,
        decimal paymentAmount,
        string? paymentCode = null,
        DateTime? paymentTime = null,
        decimal? discountAmount = null,
        Guid? createdBy = null)
    {
        if (Status != PaymentAutoStatus.Draft && Status != PaymentAutoStatus.Rejected)
            throw new BusinessException("PAYMENT_CANNOT_ADD_DETAIL", "只有待提交或已驳回状态的付款单才能添加明细");

        var detail = PaymentAutoDetail.Create(
            Id,
            debtDetailId,
            paymentAmount,
            paymentCode,
            paymentTime,
            discountAmount,
            createdBy);

        _details.Add(detail);
        return detail;
    }

    /// <summary>
    /// 移除付款明细
    /// </summary>
    public void RemoveDetail(PaymentAutoDetailId detailId)
    {
        if (Status != PaymentAutoStatus.Draft && Status != PaymentAutoStatus.Rejected)
            throw new BusinessException("PAYMENT_CANNOT_REMOVE_DETAIL", "只有待提交或已驳回状态的付款单才能移除明细");

        var detail = _details.FirstOrDefault(d => d.Id == detailId);
        if (detail != null)
        {
            detail.Delete();
        }
    }

    /// <summary>
    /// 添加供应商
    /// </summary>
    public PaymentAutoAgent AddAgent(
        Guid? agentId = null,
        string? agentName = null,
        string? accountName = null,
        string? accountNumber = null,
        string? bankName = null,
        string? bankCode = null,
        string? paymentType = null,
        string? transferRemark = null,
        Guid? createdBy = null)
    {
        if (Status != PaymentAutoStatus.Draft && Status != PaymentAutoStatus.Rejected)
            throw new BusinessException("PAYMENT_CANNOT_ADD_AGENT", "只有待提交或已驳回状态的付款单才能添加供应商");

        var agent = PaymentAutoAgent.Create(
            Id,
            agentId,
            agentName,
            accountName,
            accountNumber,
            bankName,
            bankCode,
            paymentType,
            transferRemark,
            createdBy);

        _agents.Add(agent);
        return agent;
    }

    /// <summary>
    /// 移除供应商
    /// </summary>
    public void RemoveAgent(PaymentAutoAgentId agentId)
    {
        if (Status != PaymentAutoStatus.Draft && Status != PaymentAutoStatus.Rejected)
            throw new BusinessException("PAYMENT_CANNOT_REMOVE_AGENT", "只有待提交或已驳回状态的付款单才能移除供应商");

        var agent = _agents.FirstOrDefault(a => a.Id == agentId);
        if (agent != null)
        {
            agent.Delete();
        }
    }

    /// <summary>
    /// 计算总金额
    /// </summary>
    public decimal CalculateTotalAmount()
    {
        return _details.Where(d => !d.IsDeleted).Sum(d => d.PaymentAmount);
    }

    /// <summary>
    /// 软删除
    /// </summary>
    public void Delete()
    {
        if (Status == PaymentAutoStatus.Approving)
            throw new BusinessException("PAYMENT_CANNOT_DELETE", "审批中的付款单不能删除");

        if (Status == PaymentAutoStatus.Completed)
            throw new BusinessException("PAYMENT_CANNOT_DELETE", "已完成的付款单不能删除");

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;

        foreach (var detail in _details)
        {
            detail.Delete();
        }

        foreach (var agent in _agents)
        {
            agent.Delete();
        }
    }

    /// <summary>
    /// 验证数据完整性
    /// </summary>
    public bool ValidateData()
    {
        if (string.IsNullOrWhiteSpace(Code))
            return false;

        if (string.IsNullOrWhiteSpace(CompanyName) || string.IsNullOrWhiteSpace(CompanyCode))
            return false;

        var activeDetails = _details.Where(d => !d.IsDeleted).ToList();
        if (activeDetails.Count == 0)
            return false;

        if (activeDetails.Any(d => d.PaymentAmount <= 0))
            return false;

        return true;
    }
}
