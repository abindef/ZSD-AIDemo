import request from '@/utils/request';

const baseUrl = '/v1.0/finance-backend/api';

// 批量付款单相关接口
export const paymentAutoApi = {
  // 分页查询批量付款单
  getPage: (params: any) => {
    return request({
      url: `${baseUrl}/paymentAuto/page`,
      method: 'get',
      params
    });
  },

  // 查询状态统计
  getStatusCount: (params: any) => {
    return request({
      url: `${baseUrl}/paymentAuto/statusCount`,
      method: 'get',
      params
    });
  },

  // 获取批量付款单详情
  getDetail: (id: string) => {
    return request({
      url: `${baseUrl}/paymentAuto/${id}`,
      method: 'get'
    });
  },

  // 创建批量付款单
  create: (data: any) => {
    return request({
      url: `${baseUrl}/paymentAuto`,
      method: 'post',
      data
    });
  },

  // 更新批量付款单
  update: (id: string, data: any) => {
    return request({
      url: `${baseUrl}/paymentAuto/${id}`,
      method: 'put',
      data
    });
  },

  // 删除批量付款单
  delete: (ids: string) => {
    return request({
      url: `${baseUrl}/paymentAuto/${ids}`,
      method: 'delete'
    });
  },

  // 提交审批
  submit: (id: string) => {
    return request({
      url: `${baseUrl}/paymentAuto/${id}/submit`,
      method: 'post'
    });
  },

  // 获取批量付款单明细
  getDetails: (paymentAutoItemId: string) => {
    return request({
      url: `${baseUrl}/paymentAuto/${paymentAutoItemId}/details`,
      method: 'get'
    });
  },

  // 获取批量付款单供应商信息
  getAgents: (paymentAutoItemId: string) => {
    return request({
      url: `${baseUrl}/paymentAuto/${paymentAutoItemId}/agents`,
      method: 'get'
    });
  }
};

// 应付单相关接口
export const debtApi = {
  // 查询应付单列表（用于新增/编辑时选择）
  getList: (params: any) => {
    return request({
      url: `${baseUrl}/debt/list`,
      method: 'get',
      params
    });
  },

  // 查询应付明细列表
  getDetailList: (params: any) => {
    return request({
      url: `${baseUrl}/debt/details`,
      method: 'get',
      params
    });
  }
};

// 公司相关接口
export const companyApi = {
  // 查询公司列表
  getList: (params?: any) => {
    return request({
      url: `${baseUrl}/company/list`,
      method: 'get',
      params
    });
  }
};

// 部门相关接口
export const departmentApi = {
  // 查询部门树
  getTree: (params?: any) => {
    return request({
      url: `${baseUrl}/department/tree`,
      method: 'get',
      params
    });
  }
};

// 供应商相关接口
export const agentApi = {
  // 查询供应商列表
  getList: (params?: any) => {
    return request({
      url: `${baseUrl}/agent/list`,
      method: 'get',
      params
    });
  }
};
