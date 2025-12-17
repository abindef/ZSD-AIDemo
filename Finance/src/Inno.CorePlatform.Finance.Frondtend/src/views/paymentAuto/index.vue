<template>
  <div class="app-page-container">
    <div class="app-page-header">
      <el-breadcrumb separator-class="el-icon-arrow-right">
        <el-breadcrumb-item to="/home">首页</el-breadcrumb-item>
        <el-breadcrumb-item>批量付款管理</el-breadcrumb-item>
      </el-breadcrumb>
      <div class="flex-1" />
      <inno-crud-operation :crud="crud1" :permission="crudPermission" :hiddenColumns="[]"></inno-crud-operation>
    </div>
    <div class="app-page-body" style="padding-top: 0">
      <inno-query-operation :query-list="queryList" :crud="crud1" />
      
      <!-- 最外层布局：上下分割 -->
      <inno-split-pane split="horizontal" :default-percent="50" :min-percent="30">
        <!-- 上半部分：左右分割 -->
        <template #paneL>
          <inno-split-pane split="vertical" :default-percent="50" :min-percent="30">
            <!-- 左侧：批量付款单列表 -->
            <template #paneL>
              <inno-crud-operation :crud="crud1" :permission="crudPermission" :hiddenColumns="[]" border
                hidden-opts-right rightAdjust>
                <template #opts-left>
                  <el-tabs v-model="crud1.query.status" @tab-click="tabActiveClick">
                    <el-tab-pane :label="`待提交(${statusTabCount.waitSubmitCount})`" name="0" />
                    <el-tab-pane :label="`审批中(${statusTabCount.holdingCount})`" name="1" />
                    <el-tab-pane :label="`已完成(${statusTabCount.finishedCount})`" name="2" />
                    <el-tab-pane :label="`已驳回(${statusTabCount.rejectedCount})`" name="11" />
                    <el-tab-pane :label="`全部(${statusTabCount.allCount})`" name="" />
                  </el-tabs>
                </template>
                <template #right></template>
              </inno-crud-operation>
              <inno-table-container>
                <el-table ref="tableRef" v-loading="crud1.loading" highlight-current-row
                  :header-cell-style="{ background: '#EBEEF5', color: '#909399' }" border :data="crud1.data" stripe
                  @selection-change="crud1.selectionChangeHandler" @row-click="handleRowClick"
                  @sort-change="crud1.sortChange">
                  <el-table-column type="selection" fixed="left" width="55" />
                  <el-table-column property="code" label="批量付款单号" min-width="180" show-overflow-tooltip />
                  <el-table-column property="billDate" label="单据日期" min-width="130" show-overflow-tooltip>
                    <template #default="scope">
                      {{ dateFormat(scope.row.billDate, 'YYYY-MM-DD') }}
                    </template>
                  </el-table-column>
                  <el-table-column property="companyName" label="公司" min-width="200" show-overflow-tooltip />
                  <el-table-column property="departmentNamePath" label="核算部门" min-width="200" show-overflow-tooltip />
                  <el-table-column property="totalAmount" label="总金额" min-width="120" show-overflow-tooltip align="right">
                    <template #default="scope">
                      {{ formatAmount(scope.row.totalAmount) }}
                    </template>
                  </el-table-column>
                  <el-table-column property="statusDescription" label="状态" min-width="100" show-overflow-tooltip />
                  <el-table-column property="remark" label="备注" min-width="200" show-overflow-tooltip />
                  <el-table-column property="createdBy" label="申请人" min-width="120" show-overflow-tooltip />
                </el-table>
              </inno-table-container>
              <div class="app-page-footer background">
                已选择 {{ crud1.selections.length }} 条
                <div class="flex-1" />
                <inno-crud-pagination :crud="crud1" />
              </div>
            </template>
            
            <!-- 右侧：供应商信息表格 -->
            <template #paneR>
              <div style="padding: 8px; height: 100%; box-sizing: border-box;">
                <inno-crud-operation :hiddenColumns="[]" :crud="crudAgent" style="padding: 0">
                  <template #opts-left>
                    <el-tabs class="demo-tabs">
                      <el-tab-pane label="供应商信息" name="-1"></el-tab-pane>
                    </el-tabs>
                  </template>
                </inno-crud-operation>
                <el-table :data="agentList" v-loading="agentLoading" border stripe size="small" 
                  :header-cell-style="{ background: '#EBEEF5', color: '#909399' }"
                  :empty-text="currentPaymentId ? '暂无数据' : '请先选择批量付款单'">
                  <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
                  <el-table-column prop="sum" label="付款金额" min-width="120" show-overflow-tooltip align="right">
                    <template #default="scope">
                      {{ formatAmount(scope.row.sum) }}
                    </template>
                  </el-table-column>
                  <el-table-column prop="accountInfo" label="收款账户" min-width="220" show-overflow-tooltip />
                  <el-table-column prop="remark" label="备注" min-width="140" show-overflow-tooltip />
                </el-table>
              </div>
            </template>
          </inno-split-pane>
        </template>
        
        <!-- 下半部分：付款明细表格 -->
        <template #paneR>
          <div style="padding: 8px; height: 100%; box-sizing: border-box;">
            <inno-crud-operation :hiddenColumns="[]" :crud="crudDetail" style="padding: 0">
              <template #opts-left>
                <el-tabs class="demo-tabs">
                  <el-tab-pane label="付款明细" name="-1"></el-tab-pane>
                </el-tabs>
              </template>
            </inno-crud-operation>
            <el-table :data="detailList" v-loading="detailLoading" border stripe size="small"
              :header-cell-style="{ background: '#EBEEF5', color: '#909399' }"
              :empty-text="currentPaymentId ? '暂无数据' : '请先选择批量付款单'">
              <el-table-column prop="debtCode" label="应付单号" min-width="180" show-overflow-tooltip />
              <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
              <el-table-column prop="documentType" label="应付单据类型" min-width="120" show-overflow-tooltip />
              <el-table-column prop="paymentTermType" label="账期类型" min-width="120" show-overflow-tooltip />
              <el-table-column prop="expectedPaymentDate" label="预计付款日期" min-width="130" show-overflow-tooltip>
                <template #default="scope">
                  {{ dateFormat(scope.row.expectedPaymentDate, 'YYYY-MM-DD') }}
                </template>
              </el-table-column>
              <el-table-column prop="paymentAmount" label="付款金额" min-width="120" show-overflow-tooltip align="right">
                <template #default="scope">
                  {{ formatAmount(scope.row.paymentAmount) }}
                </template>
              </el-table-column>
              <el-table-column prop="paidAmount" label="已付金额" min-width="120" show-overflow-tooltip align="right">
                <template #default="scope">
                  {{ formatAmount(scope.row.paidAmount) }}
                </template>
              </el-table-column>
              <el-table-column prop="remainingAmount" label="剩余金额" min-width="120" show-overflow-tooltip align="right">
                <template #default="scope">
                  {{ formatAmount(scope.row.remainingAmount) }}
                </template>
              </el-table-column>
            </el-table>
          </div>
        </template>
      </inno-split-pane>
    </div>

    <!-- 新增/编辑对话框 -->
    <PaymentAutoDialog 
      v-model:visible="dialogVisible" 
      :is-edit="dialogIsEdit" 
      :data="dialogData"
      @success="handleDialogSuccess" 
    />
  </div>
</template>

<script lang="ts" setup>
import { ref, onBeforeMount, onMounted, onActivated, computed, reactive } from 'vue';
import { ElTable, ElMessage, ElMessageBox } from 'element-plus';
import type { TabsPaneContext } from 'element-plus';
import CRUD, { tableDrag } from '@inno/inno-mc-vue3/lib/crud';
import { dateFormat } from '@inno/inno-mc-vue3/lib/utils/filters';
import { useRouter, useRoute } from 'vue-router';
import { paymentAutoApi } from '@/api/paymentAuto';
import type { 
  PaymentAutoItem, 
  PaymentAutoAgent, 
  DebtDetail,
  StatusTabCount 
} from '@/types/paymentAuto';
import PaymentAutoDialog from './components/PaymentAutoDialog.vue';

const route = useRoute();
const router = useRouter();

onActivated(() => {
  if (route.params.__refresh) {
    crud1.toQuery();
  }
});

// 对话框控制
const dialogVisible = ref(false);
const dialogIsEdit = ref(false);
const dialogData = ref<PaymentAutoItem | null>(null);

// 当前选中的批量付款单ID
const currentPaymentId = ref<string>('');

// 供应商信息列表
const agentList = ref<PaymentAutoAgent[]>([]);
const agentLoading = ref(false);

// 付款明细列表
const detailList = ref<DebtDetail[]>([]);
const detailLoading = ref(false);

// 表格引用
const tableRef = ref<InstanceType<typeof ElTable>>();

// 批量付款单列表配置
const crud1 = CRUD(
  {
    title: '批量付款单',
    url: '/v1.0/finance-backend/api/paymentAuto/page',
    idField: 'id',
    sort: [],
    query: {
      status: '0'
    },
    defaultForm: () => ({
      id: '',
      code: '',
      companyId: '',
      departmentIdPath: '',
      remark: ''
    }),
    method: 'get',
    crudMethod: {
      del: (ids) => paymentAutoApi.delete(ids)
    },
    userNames: ['createdBy', 'updatedBy'],
    optShow: {
      add: true,
      edit: true,
      del: true,
      download: false,
      reset: true
    },
    pageConfig: {
      pageIndex: 'page',
      pageSize: 'limit'
    },
    hooks: {
      [CRUD.HOOK.afterToAdd]: (_crud: any) => {
        dialogData.value = null;
        dialogIsEdit.value = false;
        dialogVisible.value = true;
      },
      [CRUD.HOOK.afterToEdit]: (_crud: any) => {
        dialogData.value = _crud.form;
        dialogIsEdit.value = true;
        dialogVisible.value = true;
      }
    },
    resultKey: {
      list: 'list',
      total: 'total'
    },
    props: {
      searchToggle: true
    }
  },
  {
    table: tableRef
  }
);

// 状态统计
const statusTabCount = reactive<StatusTabCount>({
  waitSubmitCount: 0,
  holdingCount: 0,
  finishedCount: 0,
  rejectedCount: 0,
  allCount: 0
});

// 加载状态统计
const loadStatusCount = () => {
  paymentAutoApi.getStatusCount(crud1.query)
    .then((res) => {
      if (res.data && res.data.data) {
        Object.assign(statusTabCount, res.data.data);
      }
    })
    .catch((err) => {
      console.error('加载状态统计失败:', err);
    });
};

// Tab切换事件
const tabActiveClick = (tab: TabsPaneContext) => {
  crud1.toQuery();
  loadStatusCount();
};

// 行点击事件
const handleRowClick = (row: PaymentAutoItem) => {
  currentPaymentId.value = row.id;
  loadAgentList(row.id);
  loadDetailList(row.id);
};

// 加载供应商信息
const loadAgentList = (paymentAutoItemId: string) => {
  agentLoading.value = true;
  paymentAutoApi.getAgents(paymentAutoItemId)
    .then((res) => {
      if (res.data && res.data.data) {
        agentList.value = res.data.data.map((item: PaymentAutoAgent) => ({
          ...item,
          accountInfo: `${item.accountName} ${item.accountNo} ${item.bankName}`
        }));
      }
    })
    .catch((err) => {
      console.error('加载供应商信息失败:', err);
      agentList.value = [];
    })
    .finally(() => {
      agentLoading.value = false;
    });
};

// 加载付款明细
const loadDetailList = (paymentAutoItemId: string) => {
  detailLoading.value = true;
  paymentAutoApi.getDetails(paymentAutoItemId)
    .then((res) => {
      if (res.data && res.data.data) {
        detailList.value = res.data.data;
      }
    })
    .catch((err) => {
      console.error('加载付款明细失败:', err);
      detailList.value = [];
    })
    .finally(() => {
      detailLoading.value = false;
    });
};

// 高级查询条件
const queryList = computed(() => [
  {
    key: 'code',
    label: '批量付款单号',
    show: true,
    props: {
      style: {
        width: '400px'
      },
      placeholder: '请输入批量付款单号'
    }
  },
  {
    key: 'companyId',
    label: '公司',
    type: 'select',
    show: true,
    labelK: 'name',
    valueK: 'id',
    dataList: [],
    props: {
      placeholder: '请选择公司'
    }
  },
  {
    key: 'agentId',
    label: '供应商',
    type: 'select',
    show: true,
    labelK: 'name',
    valueK: 'id',
    dataList: [],
    props: {
      placeholder: '请选择供应商'
    }
  },
  {
    key: 'billDate',
    endDate: 'billDateEnd',
    type: 'datetimerange',
    label: '单据日期',
    show: true,
    props: {
      placeholder: '请选择日期范围'
    }
  }
]);

// 权限配置
const crudPermission = ref({
  add: ['/paymentAuto/All'],
  edit: ['/paymentAuto/All'],
  del: ['/paymentAuto/All'],
  download: ['/paymentAuto/All']
});

// 供应商信息CRUD配置（仅用于显示）
const crudAgent = CRUD({
  title: '供应商信息',
  url: '',
  idField: 'id',
  optShow: {
    add: false,
    edit: false,
    del: false,
    download: false,
    reset: false
  },
  props: {
    searchToggle: false
  }
});

// 付款明细CRUD配置（仅用于显示）
const crudDetail = CRUD({
  title: '付款明细',
  url: '',
  idField: 'id',
  optShow: {
    add: false,
    edit: false,
    del: false,
    download: false,
    reset: false
  },
  props: {
    searchToggle: false
  }
});

// 对话框成功回调
const handleDialogSuccess = () => {
  crud1.toQuery();
  loadStatusCount();
  
  // 如果当前有选中的记录，刷新详情
  if (currentPaymentId.value) {
    loadAgentList(currentPaymentId.value);
    loadDetailList(currentPaymentId.value);
  }
};

// 金额格式化
const formatAmount = (amount: number | undefined | null): string => {
  if (amount === undefined || amount === null) return '0.00';
  return amount.toLocaleString('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

// 生命周期钩子
onBeforeMount(() => {
  crud1.toQuery();
  loadStatusCount();
});

onMounted(() => {
  tableDrag(tableRef);
});
</script>

<style scoped>
.app-page-container {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.app-page-header {
  display: flex;
  align-items: center;
  padding: 16px;
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
}

.app-page-body {
  flex: 1;
  overflow: hidden;
  padding: 16px;
}

.app-page-footer {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  border-top: 1px solid #e4e7ed;
}

.background {
  background: #fff;
}

.flex-1 {
  flex: 1;
}

:deep(.el-tabs__header) {
  margin: 0;
}

:deep(.el-tabs__nav-wrap::after) {
  display: none;
}
</style>
