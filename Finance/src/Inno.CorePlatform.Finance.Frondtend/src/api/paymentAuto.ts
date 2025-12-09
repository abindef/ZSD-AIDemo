import request from '@/utils/request';

const baseUrl = '/api/v1/PaymentAuto';

// 批量付款单状态枚举
export const PaymentAutoStatus = {
  Draft: 0, // 待提交
  Approving: 1, // 审批中
  Completed: 2, // 已完成
  Rejected: 11 // 已驳回
};

// 状态选项
export const statusOptions = [
  { id: 0, name: '待提交' },
  { id: 1, name: '审批中' },
  { id: 2, name: '已完成' },
  { id: 11, name: '已驳回' }
];

// 批量付款明细接口
export interface PaymentAutoDetailDto {
  id?: string;
  paymentAutoItemId?: string;
  debtDetailId: string;
  paymentCode?: string;
  paymentAmount: number;
  paymentTime?: string;
  discountAmount?: number;
  createdBy?: string;
  createdAt?: string;
}

// 批量付款供应商接口
export interface PaymentAutoAgentDto {
  id?: string;
  paymentAutoItemId?: string;
  agentId?: string;
  agentName?: string;
  accountName?: string;
  accountNumber?: string;
  bankName?: string;
  bankCode?: string;
  paymentType?: string;
  transferRemark?: string;
  invoiceNo?: string;
  contractNo?: string;
  crossBorderPayment?: number;
  transactionCode?: string;
  transactionRemark?: string;
  isBondedGoods?: number;
  feeBearer?: string;
  customsGoods?: string;
  attachmentIds?: string;
  isDebtTransfer?: boolean;
  debtTransferAgentId?: string;
  debtTransferAgentName?: string;
  debtTransferAccount?: string;
  debtTransferBankAccount?: string;
  debtTransferBank?: string;
  debtTransferBankName?: string;
  debtTransferBankCode?: string;
  debtTransferAttachment?: string;
  groupPaymentId?: number;
  createdBy?: string;
  createdAt?: string;
}

// 批量付款单接口
export interface PaymentAutoItemDto {
  id: string;
  code: string;
  billDate: string;
  companyId?: string;
  companyName: string;
  companyCode: string;
  remark?: string;
  overallPostscript?: string;
  status: number;
  statusDescription: string;
  oaRequestId?: string;
  oaLastApprover?: string;
  oaLastApprovalTime?: string;
  oaApprovalComment?: string;
  departmentIdPath?: string;
  departmentNamePath?: string;
  currentDepartmentId?: string;
  attachmentIds?: string;
  totalAmount: number;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
  details: PaymentAutoDetailDto[];
  agents: PaymentAutoAgentDto[];
}

// 批量付款单列表项接口
export interface PaymentAutoListItemDto {
  id: string;
  code: string;
  billDate: string;
  companyName: string;
  totalAmount: number;
  status: number;
  statusDescription: string;
  remark?: string;
  createdByName?: string;
  createdAt?: string;
}

// 状态统计接口
export interface PaymentAutoStatusCountDto {
  waitSubmitCount: number;
  holdingCount: number;
  finishedCount: number;
  rejectedCount: number;
  allCount: number;
}

// 分页结果接口
export interface PagedResult<T> {
  list: T[];
  total: number;
  page: number;
  pageSize: number;
}

// 创建批量付款单请求
export interface CreatePaymentAutoRequest {
  code: string;
  billDate: string;
  companyId?: string;
  companyName: string;
  companyCode: string;
  remark?: string;
  overallPostscript?: string;
  departmentIdPath?: string;
  departmentNamePath?: string;
  currentDepartmentId?: string;
  attachmentIds?: string;
  createdBy?: string;
  details: PaymentAutoDetailDto[];
  agents: PaymentAutoAgentDto[];
}

// 更新批量付款单请求
export interface UpdatePaymentAutoRequest extends CreatePaymentAutoRequest {
  id: string;
  updatedBy?: string;
}

// 更新状态请求
export interface UpdateStatusRequest {
  id: string;
  status: number;
  oaRequestId?: string;
  oaLastApprover?: string;
  oaLastApprovalTime?: string;
  oaApprovalComment?: string;
  updatedBy?: string;
}

// 查询参数
export interface QueryParams {
  page?: number;
  limit?: number;
  status?: number | string;
  keyword?: string;
  startDate?: string;
  endDate?: string;
}

/**
 * 获取批量付款单详情
 */
export function getPaymentAutoById(id: string) {
  return request({
    url: `${baseUrl}/${id}`,
    method: 'get'
  });
}

/**
 * 分页查询批量付款单
 */
export function getPaymentAutoPage(params: QueryParams) {
  return request({
    url: baseUrl,
    method: 'get',
    params
  });
}

/**
 * 获取状态统计
 */
export function getStatusCount(params?: Partial<QueryParams>) {
  return request({
    url: `${baseUrl}/status-count`,
    method: 'get',
    params
  });
}

/**
 * 创建批量付款单
 */
export function createPaymentAuto(data: CreatePaymentAutoRequest) {
  return request({
    url: baseUrl,
    method: 'post',
    data
  });
}

/**
 * 更新批量付款单
 */
export function updatePaymentAuto(id: string, data: UpdatePaymentAutoRequest) {
  return request({
    url: `${baseUrl}/${id}`,
    method: 'put',
    data
  });
}

/**
 * 删除批量付款单
 */
export function deletePaymentAuto(id: string) {
  return request({
    url: `${baseUrl}/${id}`,
    method: 'delete'
  });
}

/**
 * 批量删除批量付款单
 */
export function batchDeletePaymentAuto(ids: string[]) {
  return request({
    url: baseUrl,
    method: 'delete',
    data: ids
  });
}

/**
 * 更新批量付款单状态
 */
export function updatePaymentAutoStatus(id: string, data: UpdateStatusRequest) {
  return request({
    url: `${baseUrl}/${id}/status`,
    method: 'put',
    data
  });
}
