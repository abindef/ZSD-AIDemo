import request from '@/utils/request';

const uri = 'bds-backend';
// 管理类别
export const manageModeGroup = [
  {
    id: 0,
    name: '按批'
  },
  {
    id: 1,
    name: '按个'
  }
];
// 是否
export const whetherGroup = [
  {
    id: 1,
    name: '是'
  },
  {
    id: 0,
    name: '否'
  }
];
// 供货类别
export const agentPropertyGroup = [
  {
    id: 1,
    name: '注册人/备案人'
  },
  {
    id: 2,
    name: '医疗器械经营企业'
  },
  {
    id: 3,
    name: '其他'
  },
  {
    id: 4,
    name: '其他非贸易类'
  }
];
// 存储条件
export const storageConditionGroup = [
  {
    id: '111',
    name: '常温'
  },
  {
    id: '112',
    name: '冷藏'
  },
  {
    id: '113',
    name: '冷冻'
  },
  {
    id: '114',
    name: '恒温'
  },
  {
    id: '115',
    name: '阴凉'
  },
  {
    id: '999',
    name: '见详细描述'
  }
];

// 包装层级
export const packLevelGroup = [
  {
    id: '0',
    name: '0 (最小单位)'
  },
  {
    id: '1',
    name: '1'
  },
  {
    id: '2',
    name: '2'
  },
  {
    id: '3',
    name: '3'
  },
  {
    id: '4',
    name: '4'
  },
  {
    id: '5',
    name: '5'
  },
  {
    id: '6',
    name: '6'
  },
  {
    id: '7',
    name: '7'
  },
  {
    id: '8',
    name: '8'
  },
  {
    id: '9',
    name: '9'
  }
];

export function queryProductNameMeta(data: { [key: string]: any }) {
  return request({
    url: `${uri}/api/productNames/meta`,
    method: 'get',
    params: data
  });
}

export function queryPackUnitMeta(data: { [key: string]: any }) {
  return request({
    url: `${uri}/api/productNames/meta`,
    method: 'get',
    data
  });
}

export function queryCompanyMeta(data: { [key: string]: any }) {
  return request({
    url: `${uri}/api/companies/meta`,
    method: 'get',
    data
  });
}
