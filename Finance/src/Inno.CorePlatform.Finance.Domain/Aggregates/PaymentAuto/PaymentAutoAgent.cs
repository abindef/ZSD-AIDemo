using Inno.CorePlatform.Finance.Domain.Common;

namespace Inno.CorePlatform.Finance.Domain.Aggregates.PaymentAuto;

/// <summary>
/// 批量付款供应商实体
/// </summary>
public class PaymentAutoAgent : Entity<PaymentAutoAgentId>
{
    private PaymentAutoAgent() { }

    /// <summary>
    /// 批量付款单Id
    /// </summary>
    public PaymentAutoItemId PaymentAutoItemId { get; private set; } = null!;

    /// <summary>
    /// 供应商Id
    /// </summary>
    public Guid? AgentId { get; private set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string? AgentName { get; private set; }

    /// <summary>
    /// 账号名称
    /// </summary>
    public string? AccountName { get; private set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string? AccountNumber { get; private set; }

    /// <summary>
    /// 开户行名称
    /// </summary>
    public string? BankName { get; private set; }

    /// <summary>
    /// 开户行编码
    /// </summary>
    public string? BankCode { get; private set; }

    /// <summary>
    /// 支付类型
    /// </summary>
    public string? PaymentType { get; private set; }

    /// <summary>
    /// 转账附言
    /// </summary>
    public string? TransferRemark { get; private set; }

    /// <summary>
    /// 发票号
    /// </summary>
    public string? InvoiceNo { get; private set; }

    /// <summary>
    /// 合同号
    /// </summary>
    public string? ContractNo { get; private set; }

    /// <summary>
    /// 境外/跨境支付 0:否,1:是
    /// </summary>
    public int? CrossBorderPayment { get; private set; }

    /// <summary>
    /// 交易编码
    /// </summary>
    public string? TransactionCode { get; private set; }

    /// <summary>
    /// 交易附言
    /// </summary>
    public string? TransactionRemark { get; private set; }

    /// <summary>
    /// 本笔款项是否为保税货物下推 0:否,1:是
    /// </summary>
    public int? IsBondedGoods { get; private set; }

    /// <summary>
    /// 国内外费用承担方 A:汇款人,B:收款人,C:共同人
    /// </summary>
    public string? FeeBearer { get; private set; }

    /// <summary>
    /// 海关进口货物报关商品
    /// </summary>
    public string? CustomsGoods { get; private set; }

    /// <summary>
    /// 附件Ids
    /// </summary>
    public string? AttachmentIds { get; private set; }

    /// <summary>
    /// 是否债务转移
    /// </summary>
    public bool? IsDebtTransfer { get; private set; }

    /// <summary>
    /// 债务转移的供应商Id
    /// </summary>
    public Guid? DebtTransferAgentId { get; private set; }

    /// <summary>
    /// 债务转移的供应商
    /// </summary>
    public string? DebtTransferAgentName { get; private set; }

    /// <summary>
    /// 债务转移的账户
    /// </summary>
    public string? DebtTransferAccount { get; private set; }

    /// <summary>
    /// 债务转移的银行账号
    /// </summary>
    public string? DebtTransferBankAccount { get; private set; }

    /// <summary>
    /// 债务转移的银行
    /// </summary>
    public string? DebtTransferBank { get; private set; }

    /// <summary>
    /// 债务转移的银行名称
    /// </summary>
    public string? DebtTransferBankName { get; private set; }

    /// <summary>
    /// 债务转移的银行代码
    /// </summary>
    public string? DebtTransferBankCode { get; private set; }

    /// <summary>
    /// 债务转移的附件
    /// </summary>
    public string? DebtTransferAttachment { get; private set; }

    /// <summary>
    /// 集团业务付款ID
    /// </summary>
    public int? GroupPaymentId { get; private set; }

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
    /// 工厂方法 - 创建批量付款供应商
    /// </summary>
    public static PaymentAutoAgent Create(
        PaymentAutoItemId paymentAutoItemId,
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
        Guard.Against.Null(paymentAutoItemId, nameof(paymentAutoItemId));

        return new PaymentAutoAgent
        {
            Id = PaymentAutoAgentId.Create(),
            PaymentAutoItemId = paymentAutoItemId,
            AgentId = agentId,
            AgentName = agentName,
            AccountName = accountName,
            AccountNumber = accountNumber,
            BankName = bankName,
            BankCode = bankCode,
            PaymentType = paymentType,
            TransferRemark = transferRemark,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    /// <summary>
    /// 更新供应商信息
    /// </summary>
    public void Update(
        Guid? agentId = null,
        string? agentName = null,
        string? accountName = null,
        string? accountNumber = null,
        string? bankName = null,
        string? bankCode = null,
        string? paymentType = null,
        string? transferRemark = null,
        string? invoiceNo = null,
        string? contractNo = null,
        int? crossBorderPayment = null,
        string? transactionCode = null,
        string? transactionRemark = null,
        int? isBondedGoods = null,
        string? feeBearer = null,
        string? customsGoods = null,
        string? attachmentIds = null,
        bool? isDebtTransfer = null,
        Guid? debtTransferAgentId = null,
        string? debtTransferAgentName = null,
        string? debtTransferAccount = null,
        string? debtTransferBankAccount = null,
        string? debtTransferBank = null,
        string? debtTransferBankName = null,
        string? debtTransferBankCode = null,
        string? debtTransferAttachment = null,
        int? groupPaymentId = null,
        Guid? updatedBy = null)
    {
        AgentId = agentId;
        AgentName = agentName;
        AccountName = accountName;
        AccountNumber = accountNumber;
        BankName = bankName;
        BankCode = bankCode;
        PaymentType = paymentType;
        TransferRemark = transferRemark;
        InvoiceNo = invoiceNo;
        ContractNo = contractNo;
        CrossBorderPayment = crossBorderPayment;
        TransactionCode = transactionCode;
        TransactionRemark = transactionRemark;
        IsBondedGoods = isBondedGoods;
        FeeBearer = feeBearer;
        CustomsGoods = customsGoods;
        AttachmentIds = attachmentIds;
        IsDebtTransfer = isDebtTransfer;
        DebtTransferAgentId = debtTransferAgentId;
        DebtTransferAgentName = debtTransferAgentName;
        DebtTransferAccount = debtTransferAccount;
        DebtTransferBankAccount = debtTransferBankAccount;
        DebtTransferBank = debtTransferBank;
        DebtTransferBankName = debtTransferBankName;
        DebtTransferBankCode = debtTransferBankCode;
        DebtTransferAttachment = debtTransferAttachment;
        GroupPaymentId = groupPaymentId;
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
