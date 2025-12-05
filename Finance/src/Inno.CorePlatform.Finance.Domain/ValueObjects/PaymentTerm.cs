using Inno.CorePlatform.Finance.Domain.Common;

namespace Inno.CorePlatform.Finance.Domain.ValueObjects;

/// <summary>
/// 账期值对象
/// </summary>
public record PaymentTerm
{
    /// <summary>
    /// 账期天数
    /// </summary>
    public int Days { get; }

    /// <summary>
    /// 账期描述
    /// </summary>
    public string Description { get; }

    private PaymentTerm(int days, string description)
    {
        Days = days;
        Description = description;
    }

    public static PaymentTerm Create(int days, string? description = null)
    {
        Guard.Against.Negative(days, nameof(days));

        var desc = description ?? $"{days}天账期";
        return new PaymentTerm(days, desc);
    }

    /// <summary>
    /// 计算到期日期
    /// </summary>
    public DateTime CalculateDueDate(DateTime startDate)
    {
        return startDate.AddDays(Days);
    }

    /// <summary>
    /// 现结
    /// </summary>
    public static PaymentTerm Immediate => new(0, "现结");

    /// <summary>
    /// 月结30天
    /// </summary>
    public static PaymentTerm Net30 => new(30, "月结30天");

    /// <summary>
    /// 月结60天
    /// </summary>
    public static PaymentTerm Net60 => new(60, "月结60天");

    /// <summary>
    /// 月结90天
    /// </summary>
    public static PaymentTerm Net90 => new(90, "月结90天");
}
