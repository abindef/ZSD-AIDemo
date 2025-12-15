// 批量付款单状态枚举
export enum PaymentAutoStatus {
  WaitSubmit = 0,    // 待提交
  Holding = 1,       // 审批中
  Finished = 2,      // 已完成
  Rejected = 11      // 已驳回
}

// 批量付款单状态描述映射
export const PaymentAutoStatusMap: Record<number, string> = {
  [PaymentAutoStatus.WaitSubmit]: '待提交',
  [PaymentAutoStatus.Holding]: '审批中',
  [PaymentAutoStatus.Finished]: '已完成',
  [PaymentAutoStatus.Rejected]: '已驳回'
};

// 批量付款单表
export interface PaymentAutoItem {
  id: string;
  code: string;
  billDate: string;
  companyId: string;
  companyName: string;
  companyCode: string;
  departmentIdPath: string;
  departmentNamePath: string;
  departmentCurrentId: string;
  status: PaymentAutoStatus;
  statusDescription?: string;
  totalAmount: number;
  remark?: string;
  attachmentIds?: string;
  oaRequestId?: string;
  isDeleted: boolean;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
  updatedBy?: string;
}

// 批量付款明细表
export interface PaymentAutoDetail {
  id: string;
  paymentAutoItemId: string;
  debtDetailId: string;
  paymentAmount: number;
  paymentNo?: string;
  paymentTime?: string;
  isDeleted: boolean;
}

// 批量付款供应商表
export interface PaymentAutoAgent {
  id: string;
  paymentAutoItemId: string;
  agentId: string;
  agentName: string;
  accountName: string;
  accountNo: string;
  bankName: string;
  bankCode: string;
  sum?: number;
  accountInfo?: string;
  remark?: string;
  isDeleted: boolean;
}

// 应付表
export interface Debt {
  id: string;
  code: string;
  companyId: string;
  companyName: string;
  companyCode: string;
  departmentIdPath: string;
  departmentNamePath: string;
  departmentCurrentId: string;
  agentId: string;
  agentName: string;
  documentType: string;
  businessUnit?: string;
  customer?: string;
  currency?: string;
  purchaseContractNo?: string;
  projectName?: string;
}

// 应付明细表
export interface DebtDetail {
  id: string;
  debtId: string;
  sequenceNo: number;
  paymentTermType: string;
  expectedPaymentDate: string;
  paymentAmount: number;
  paidAmount: number;
  remainingAmount: number;
  // 扩展字段（用于显示）
  debtCode?: string;
  agentName?: string;
  documentType?: string;
}

// 供应商表
export interface Agent {
  id: string;
  name: string;
  code: string;
  bankAccountName: string;
  bankAccountNo: string;
  bankName: string;
  bankCode: string;
}

// 公司
export interface Company {
  id: string;
  name: string;
  code: string;
}

// 部门
export interface Department {
  id: string;
  name: string;
  code: string;
  parentId?: string;
  idPath: string;
  namePath: string;
  children?: Department[];
}

// 状态统计
export interface StatusTabCount {
  waitSubmitCount: number;
  holdingCount: number;
  finishedCount: number;
  rejectedCount: number;
  allCount: number;
}

// 批量付款单表单数据
export interface PaymentAutoFormData {
  id?: string;
  companyId: string;
  companyName: string;
  departmentIdPath: string;
  departmentNamePath: string;
  departmentCurrentId: string;
  remark?: string;
  selectedDebtDetails: DebtDetail[];
}
