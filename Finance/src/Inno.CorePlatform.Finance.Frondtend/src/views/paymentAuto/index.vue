<template>
  <div class="app-page-container">
    <div class="app-page-header">
      <el-breadcrumb separator-class="el-icon-arrow-right">
        <el-breadcrumb-item to="/home">首页</el-breadcrumb-item>
        <el-breadcrumb-item>批量付款管理</el-breadcrumb-item>
      </el-breadcrumb>
      <div class="flex-1" />
      <inno-crud-operation :crud="crud" :hiddenColumns="[]" hidden-opts-left></inno-crud-operation>
    </div>
    <div class="app-page-body" style="padding-top: 0">
      <inno-query-operation :query-list="queryList" :crud="crud" />
      <!-- 主表格区域 -->
      <inno-split-pane split="horizontal" :default-percent="60" :min-percent="20">
        <!-- 上半部分：左右分割 -->
        <template #paneL>
          <inno-split-pane split="vertical" :default-percent="60" :min-percent="20">
            <!-- 左边：主表格 -->
            <template #paneL>
              <inno-crud-operation :crud="crud" :permission="crudPermission" :hiddenColumns="[]" border
                hidden-opts-right rightAdjust>
                <template #opts-left>
                  <el-tabs v-model="crud.query.status" @tab-click="tabActiveClick">
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
                <el-table ref="tableRef" v-loading="crud.loading" highlight-current-row
                  :header-cell-style="{ background: '#EBEEF5', color: '#909399' }" border :data="crud.data" stripe
                  @selection-change="crud.selectionChangeHandler" @row-click="handleRowClick"
                  @sort-change="crud.sortChange">
                  <el-table-column type="selection" fixed="left" width="55" />
                  <el-table-column property="code" label="批量付款单号" min-width="180" show-overflow-tooltip />
                  <el-table-column property="billDate" label="单据日期" min-width="130" show-overflow-tooltip>
                    <template #default="scope">
                      {{ dateFormat(scope.row.billDate, 'YYYY-MM-DD') }}
                    </template>
                  </el-table-column>
                  <el-table-column property="companyName" label="公司" min-width="200" show-overflow-tooltip />
                  <el-table-column property="totalAmount" label="总金额" min-width="120" show-overflow-tooltip>
                    <template #default="scope">
                      {{ formatMoney(scope.row.totalAmount) }}
                    </template>
                  </el-table-column>
                  <el-table-column property="statusDescription" label="状态" min-width="100" show-overflow-tooltip>
                    <template #default="scope">
                      <el-tag :type="getStatusType(scope.row.status)">
                        {{ scope.row.statusDescription }}
                      </el-tag>
                    </template>
                  </el-table-column>
                  <el-table-column property="remark" label="备注" min-width="200" show-overflow-tooltip />
                  <el-table-column property="createdByName" label="申请人" min-width="120" show-overflow-tooltip />
                  <el-table-column property="createdAt" label="创建时间" min-width="160" show-overflow-tooltip>
                    <template #default="scope">
                      {{ dateFormat(scope.row.createdAt, 'YYYY-MM-DD HH:mm:ss') }}
                    </template>
                  </el-table-column>
                  <el-table-column label="操作" width="150" fixed="right">
                    <template #default="scope">
                      <el-button type="primary" link size="small" @click="handleEdit(scope.row)">
                        编辑
                      </el-button>
                      <el-button type="danger" link size="small" @click="handleDelete(scope.row)">
                        删除
                      </el-button>
                    </template>
                  </el-table-column>
                </el-table>
              </inno-table-container>
              <div class="app-page-footer background">
                已选择 {{ crud.selections.length }} 条
                <div class="flex-1" />
                <inno-crud-pagination :crud="crud" />
              </div>
            </template>
            <!-- 右边：供应商信息表格 -->
            <template #paneR>
              <div style="padding: 8px; height: 100%; box-sizing: border-box">
                <inno-crud-operation :hiddenColumns="[]" :crud="agentCrud" style="padding: 0">
                  <template #opts-left>
                    <el-tabs class="demo-tabs">
                      <el-tab-pane label="供应商信息" name="-1"></el-tab-pane>
                    </el-tabs>
                  </template>
                </inno-crud-operation>
                <el-table :data="agentList" v-loading="agentLoading" border stripe size="small">
                  <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
                  <el-table-column prop="totalAmount" label="付款金额" min-width="100" show-overflow-tooltip>
                    <template #default="scope">
                      {{ formatMoney(scope.row.totalAmount) }}
                    </template>
                  </el-table-column>
                  <el-table-column prop="accountInfo" label="收款账户" min-width="220" show-overflow-tooltip>
                    <template #default="scope">
                      {{ scope.row.accountName }} - {{ scope.row.accountNumber }}
                    </template>
                  </el-table-column>
                  <el-table-column prop="bankName" label="开户行" min-width="180" show-overflow-tooltip />
                  <el-table-column prop="transferRemark" label="转账附言" min-width="140" show-overflow-tooltip />
                </el-table>
              </div>
            </template>
          </inno-split-pane>
        </template>
        <!-- 下半部分：付款明细 -->
        <template #paneR>
          <div style="padding: 8px">
            <div style="padding: 8px; height: 100%; box-sizing: border-box">
              <inno-crud-operation :hiddenColumns="[]" :crud="crud" style="padding: 0">
                <template #opts-left>
                  <el-tabs class="demo-tabs">
                    <el-tab-pane label="付款明细" name="-1"></el-tab-pane>
                  </el-tabs>
                </template>
              </inno-crud-operation>
              <el-table :data="detailList" v-loading="detailLoading" border stripe size="small">
                <el-table-column prop="paymentCode" label="付款单号" min-width="160" show-overflow-tooltip />
                <el-table-column prop="paymentAmount" label="付款金额" min-width="120" show-overflow-tooltip>
                  <template #default="scope">
                    {{ formatMoney(scope.row.paymentAmount) }}
                  </template>
                </el-table-column>
                <el-table-column prop="paymentTime" label="付款时间" min-width="160" show-overflow-tooltip>
                  <template #default="scope">
                    {{ scope.row.paymentTime ? dateFormat(scope.row.paymentTime, 'YYYY-MM-DD HH:mm:ss') : '-' }}
                  </template>
                </el-table-column>
                <el-table-column prop="discountAmount" label="折扣金额" min-width="120" show-overflow-tooltip>
                  <template #default="scope">
                    {{ scope.row.discountAmount ? formatMoney(scope.row.discountAmount) : '-' }}
                  </template>
                </el-table-column>
              </el-table>
            </div>
          </div>
        </template>
      </inno-split-pane>
    </div>

    <!-- 新增/编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="80%" :close-on-click-modal="false" destroy-on-close>
      <payment-auto-form v-if="dialogVisible" ref="formRef" :data="currentRow" :is-edit="isEdit"
        @success="handleFormSuccess" @cancel="dialogVisible = false" />
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, onBeforeMount, onMounted, computed, reactive } from 'vue';
import { ElTable, ElMessage, ElMessageBox, TabsPaneContext } from 'element-plus';
import CRUD, { tableDrag } from '@inno/inno-mc-vue3/lib/crud';
import { dateFormat } from '@inno/inno-mc-vue3/lib/utils/filters';
import service from '@/utils/request';
import {
  getPaymentAutoById,
  getPaymentAutoPage,
  getStatusCount,
  deletePaymentAuto,
  batchDeletePaymentAuto,
  PaymentAutoListItemDto,
  PaymentAutoItemDto,
  PaymentAutoDetailDto,
  PaymentAutoAgentDto
} from '@/api/paymentAuto';
import PaymentAutoForm from './components/PaymentAutoForm.vue';
const crudPermission = ref({
  add: ['/paymentAuto/All'],
  edit: ['/paymentAuto/All'],
  del: ['/paymentAuto/All'],
  download: ['/paymentAuto/All']
});
const tableRef = ref<InstanceType<typeof ElTable>>();
const formRef = ref();

// 对话框状态
const dialogVisible = ref(false);
const dialogTitle = ref('新增批量付款单');
const isEdit = ref(false);
const currentRow = ref<PaymentAutoItemDto | null>(null);

// 供应商和明细数据
const agentList = ref<PaymentAutoAgentDto[]>([]);
const agentLoading = ref(false);
const detailList = ref<PaymentAutoDetailDto[]>([]);
const detailLoading = ref(false);

// CRUD配置
const crud = CRUD(
  {
    title: '批量付款单',
    url: '/api/v1/PaymentAuto',
    idField: 'id',
    sort: [],
    query: {
      status: ''
    },
    defaultForm: () => ({
      id: '',
      code: '',
      billDate: '',
      companyId: '',
      companyName: '',
      companyCode: '',
      remark: '',
      details: [],
      agents: []
    }),
    method: 'get',
    crudMethod: {
      del: (ids: string) => deletePaymentAuto(ids),
      delAll: (ids: string[]) => batchDeletePaymentAuto(ids)
    },
    optShow: {
      add: true,
      edit: true,
      del: true,
      download: true,
      reset: true
    },
    pageConfig: {
      pageIndex: 'page',
      pageSize: 'limit'
    },
    hooks: {
      beforeToAdd: () => {
        isEdit.value = false;
        currentRow.value = null;
        dialogTitle.value = '新增批量付款单';
        dialogVisible.value = true;
        return false; // 阻止默认行为
      },
      beforeToEdit: () => {
        return false; // 阻止默认行为，使用自定义编辑
      }
    },
    resultKey: {
      list: 'data.list',
      total: 'data.total'
    },
    props: {
      searchToggle: true
    }
  },
  {
    table: tableRef
  }
);

// 供应商CRUD（仅用于显示）
const agentCrud = CRUD(
  {
    title: '供应商信息',
    url: '',
    idField: 'id',
    sort: [],
    query: {},
    defaultForm: () => ({ id: '' }),
    method: 'get',
    crudMethod: {},
    optShow: {
      add: false,
      edit: false,
      del: false,
      download: false,
      reset: false
    },
    pageConfig: {
      pageIndex: 'page',
      pageSize: 'limit'
    },
    resultKey: {
      list: 'list',
      total: 'total'
    },
    props: {
      searchToggle: false
    }
  },
  {}
);

// 状态统计
const statusTabCount = reactive({
  waitSubmitCount: 0,
  holdingCount: 0,
  finishedCount: 0,
  rejectedCount: 0,
  allCount: 0
});

// 加载状态统计
const loadStatusCount = async () => {
  try {
    const res = await getStatusCount({
      keyword: crud.query.keyword,
      startDate: crud.query.startDate,
      endDate: crud.query.endDate
    });
    if (res.data?.data) {
      Object.assign(statusTabCount, res.data.data);
    }
  } catch (error) {
    console.error('加载状态统计失败:', error);
  }
};

// 查询条件
const queryList = computed(() => [
  {
    key: 'keyword',
    label: '关键字',
    show: true,
    props: {
      placeholder: '单号/公司名称',
      style: { width: '200px' }
    }
  },
  {
    key: 'startDate',
    endDate: 'endDate',
    type: 'datetimerange',
    label: '单据日期',
    props: {
      'disabled-date': (date: Date) => date.getTime() > Date.now()
    },
    show: true
  }
]);

// Tab切换
const tabActiveClick = (tab: TabsPaneContext) => {
  crud.toQuery();
  loadStatusCount();
};

// 行点击事件
const handleRowClick = async (row: PaymentAutoListItemDto) => {
  try {
    agentLoading.value = true;
    detailLoading.value = true;

    const res = await getPaymentAutoById(row.id);
    if (res.data?.data) {
      const data = res.data.data as PaymentAutoItemDto;
      agentList.value = data.agents || [];
      detailList.value = data.details || [];
    }
  } catch (error) {
    console.error('加载详情失败:', error);
  } finally {
    agentLoading.value = false;
    detailLoading.value = false;
  }
};

// 编辑
const handleEdit = async (row: PaymentAutoListItemDto) => {
  try {
    const res = await getPaymentAutoById(row.id);
    if (res.data?.data) {
      currentRow.value = res.data.data;
      isEdit.value = true;
      dialogTitle.value = '编辑批量付款单';
      dialogVisible.value = true;
    }
  } catch (error) {
    ElMessage.error('加载数据失败');
  }
};

// 删除
const handleDelete = async (row: PaymentAutoListItemDto) => {
  try {
    await ElMessageBox.confirm('确定要删除该批量付款单吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await deletePaymentAuto(row.id);
    ElMessage.success('删除成功');
    crud.toQuery();
    loadStatusCount();
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '删除失败');
    }
  }
};

// 表单提交成功
const handleFormSuccess = () => {
  dialogVisible.value = false;
  crud.toQuery();
  loadStatusCount();
};

// 格式化金额
const formatMoney = (value: number) => {
  if (value === null || value === undefined) return '-';
  return value.toLocaleString('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

// 获取状态标签类型
const getStatusType = (status: number) => {
  switch (status) {
    case 0:
      return 'info';
    case 1:
      return 'warning';
    case 2:
      return 'success';
    case 11:
      return 'danger';
    default:
      return 'info';
  }
};

onBeforeMount(() => {
  crud.toQuery();
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
  padding: 12px 16px;
  background: #fff;
  border-bottom: 1px solid #e4e7ed;
}

.app-page-body {
  flex: 1;
  overflow: hidden;
  padding: 16px;
  background: #f5f7fa;
}

.app-page-footer {
  display: flex;
  align-items: center;
  padding: 12px 16px;
}

.flex-1 {
  flex: 1;
}

.background {
  background: #fff;
}
</style>
