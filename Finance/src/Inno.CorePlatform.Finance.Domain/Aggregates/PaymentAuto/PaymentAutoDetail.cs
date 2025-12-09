using Inno.CorePlatform.Finance.Domain.Common;

namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款明细实体
/// </summary>
public class PaymentAutoDetail : Entity<PaymentAutoDetailId>
{
    private PaymentAutoDetail() { }

    /// <summary>
    /// 批量付款单Id
    /// </summary>
    public PaymentAutoItemId PaymentAutoItemId { get; private set; } = null!;

    /// <summary>
    /// 应付付款计划明细Id
    /// </summary>
    public Guid DebtDetailId { get; private set; }

    /// <summary>
    /// 付款单号
    /// </summary>
    public string? PaymentCode { get; private set; }

    /// <summary>
    /// 付款金额
    /// </summary>
    public decimal PaymentAmount { get; private set; }

    /// <summary>
    /// 付款时间
    /// </summary>
    public DateTime? PaymentTime { get; private set; }

    /// <summary>
    /// 限定折扣金额
    /// </summary>
    public decimal? DiscountAmount { get; private set; }

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

    /// <summary>
    /// 工厂方法 - 创建批量付款明细
    /// </summary>
    public static PaymentAutoDetail Create(
        PaymentAutoItemId paymentAutoItemId,
        Guid debtDetailId,
        decimal paymentAmount,
        string? paymentCode = null,
        DateTime? paymentTime = null,
        decimal? discountAmount = null,
        Guid? createdBy = null)
    {
        Guard.Against.Null(paymentAutoItemId, nameof(paymentAutoItemId));
        Guard.Against.NegativeOrZero(paymentAmount, nameof(paymentAmount));

        return new PaymentAutoDetail
        {
            Id = PaymentAutoDetailId.Create(),
            PaymentAutoItemId = paymentAutoItemId,
            DebtDetailId = debtDetailId,
            PaymentCode = paymentCode,
            PaymentAmount = paymentAmount,
            PaymentTime = paymentTime,
            DiscountAmount = discountAmount,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    /// <summary>
    /// 更新明细信息
    /// </summary>
    public void Update(
        decimal paymentAmount,
        string? paymentCode = null,
        DateTime? paymentTime = null,
        decimal? discountAmount = null,
        Guid? updatedBy = null)
    {
        Guard.Against.NegativeOrZero(paymentAmount, nameof(paymentAmount));

        PaymentAmount = paymentAmount;
        PaymentCode = paymentCode;
        PaymentTime = paymentTime;
        DiscountAmount = discountAmount;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 软删除
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
