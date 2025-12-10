<template>
  <div class="app-page-container">
    <div class="app-page-header">
      <el-breadcrumb separator-class="el-icon-arrow-right">
        <el-breadcrumb-item to="/home">首页</el-breadcrumb-item>
        <el-breadcrumb-item>{{ $route.meta.name }}</el-breadcrumb-item>
      </el-breadcrumb>
      <div class="flex-1" />
      <inno-crud-operation :crud="crud1" :hiddenColumns="[]" hidden-opts-left></inno-crud-operation>
    </div>
    <div class="app-page-body" style="padding-top: 0">
      <inno-query-operation :query-list="queryList" :crud="crud1" />
      <!--最外层布局-->
      <inno-split-pane split="horizontal" :default-percent="60" :min-percent="20">
        <!-- 上半部分：再左右分割，左侧为现有表格区域，占宽度 60% -->
        <template #paneL>
          <inno-split-pane split="vertical" :default-percent="60" :min-percent="20">
            <!--左边：表格区域，占宽度 60%-->
            <template #paneL>
              <inno-crud-operation :crud="crud1" :permission="crudPermission" :hiddenColumns="[]" border
                hidden-opts-right rightAdjust>
                <template #opts-left>
                  <el-tabs v-model="crud1.query.status" @click="tabActiveClick">
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
                  @selection-change="crud1.selectionChangeHandler" @row-click="crud1.rowClick"
                  @sort-change="crud1.sortChange">
                  <el-table-column type="selection" fixed="left" width="55" />
                  <el-table-column property="code" label="批量付款单号" min-width="180" show-overflow-tooltip />
                  <el-table-column property="billDate" label="单据日期" min-width="130" show-overflow-tooltip>
                    <template #default="scope">
                      {{ dateFormat(scope.row.billDate, 'YYYY-MM-DD') }}
                    </template>
                  </el-table-column>
                  <el-table-column property="companyName" label="公司" min-width="200" show-overflow-tooltip />
                  <el-table-column property="sumValue" label="总金额" min-width="120" show-overflow-tooltip />
                  <el-table-column property="statusDescription" label="状态" min-width="100" show-overflow-tooltip />
                  <el-table-column property="probablyPayTime" label="预计付款日期" min-width="150" show-overflow-tooltip />
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
            <!--右边，表格区域-->
            <template #paneR>
              <div style="padding: 8px; height: 100%; box-sizing: border-box;">
                <inno-crud-operation :hiddenColumns="[]" :crud="crud2" style="padding: 0">
                  <template #opts-left>
                    <el-tabs class="demo-tabs">
                      <el-tab-pane :label="`供应商信息`" name="-1"></el-tab-pane>
                    </el-tabs>
                  </template>
                </inno-crud-operation>
                <el-table :data="crud2.data" v-loading="crud2.loading" border stripe size="small">
                  <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
                  <el-table-column prop="sum" label="付款金额" min-width="100" show-overflow-tooltip />
                  <el-table-column prop="accountInfo" label="收款账户" min-width="220" show-overflow-tooltip />
                  <el-table-column prop="remark" label="备注" min-width="140" show-overflow-tooltip />
                </el-table>
              </div>
            </template>
          </inno-split-pane>
        </template>
        <!-- 下半部分内容占位，后续可替换为业务组件 -->
        <template #paneR>
          <div style="padding: 8px;">
            <div style="padding: 8px; height: 100%; box-sizing: border-box;">
              <inno-crud-operation :hiddenColumns="[]" :crud="crud1" style="padding: 0">
                <template #opts-left>
                  <el-tabs class="demo-tabs">
                    <el-tab-pane :label="`供应商信息`" name="-1"></el-tab-pane>
                  </el-tabs>
                </template>
              </inno-crud-operation>
              <el-table :data="paymentList" border stripe size="small">
                <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
                <el-table-column prop="sum" label="付款金额" min-width="100" show-overflow-tooltip />
                <el-table-column prop="accountInfo" label="收款账户" min-width="220" show-overflow-tooltip />
                <el-table-column prop="remark" label="备注" min-width="140" show-overflow-tooltip />
              </el-table>
            </div>
          </div>
        </template>
      </inno-split-pane>
    </div>

    <!-- CrudDialog Component -->
    <CrudDialog v-model:visible="dialogVisible" :is-edit="dialogIsEdit" :data="dialogData"
      @success="handleDialogSuccess" />
  </div>
</template>

<script lang="tsx" setup>
/* eslint-disable */
import {
  ref,
  onBeforeMount,
  onMounted,
  onActivated,
  computed,
  reactive
} from 'vue';
import { ElTable, TabsPaneContext } from 'element-plus';
import { ElMessage, ElMessageBox } from 'element-plus';
import CRUD, { tableDrag } from '@inno/inno-mc-vue3/lib/crud';
import { dateFormat } from '@inno/inno-mc-vue3/lib/utils/filters';
import service from '@/utils/request';
import { useRouter, useRoute } from 'vue-router';
import CrudDialog from './components/CrudDialog.vue';

const route = useRoute();
onActivated(() => {
  if (route.params.__refresh) {
    crud1.toQuery();
  }
});
// Dialog visibility control
const dialogVisible = ref(false);
const dialogIsEdit = ref(false);
const dialogData = ref<any>(null);
const paymentList = ref<any[]>([]);

//------------表格一
const table1 = ref<InstanceType<typeof ElTable>>();
//列表配置查询
const crud1 = CRUD(
  {
    title: '应用',
    url: '/v1.0/sell-backend/api/saleout/page',
    downloadUrl: '/api/agents/export', // 如果不传则： '/api/agents/' + ‘download’
    idField: 'id',
    sort: [],
    query: {
      status: '1',
      saleType: '1'
    },
    defaultForm: () => {
      return {
        id: '',
        tabs: [],
        dics: [],
        customerId: '',
        customerPersonId: '',
        customerAddress: { contact: '', address: '', contactWay: '' },
        saleType: '1',
        saleProp: '',
        patienInfo: { name: '', sex: '', patienNo: '', bedNo: '' },
        description: ''
      };
    },
    method: 'get',
    crudMethod: {
      del: (ids) => service.delete(`/v1.0/sell-backend/api/saleout/${ids}`),
      edit: (data) => {
        dialogData.value = data;
        dialogIsEdit.value = true;
        dialogVisible.value = true;
        return Promise.resolve();
      },
      add: (data) => {
        dialogData.value = null;
        dialogIsEdit.value = false;
        dialogVisible.value = true;
        return Promise.resolve();
      }
    },
    userNames: ['createdBy', 'updatedBy', 'disabledBy'],
    optShow: {
      add: true,
      edit: true,
      del: true,
      download: true,
      reset: true
    },
    pageConfig: {
      // 分页参数按项目可以单独配置
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
    table: table1
  }
);

//状态统计实体
const statusTabCount = reactive({
  waitSubmitCount: 0,
  holdingCount: 0,
  finishedCount: 0,
  rejectedCount: 0,
  allCount: 0
});
//状态统计方法
const loadTabCountOfTable1 = () => {
  service
    .get('/v1.0/sell-backend/api/saleout/querySaleStatusTabCount', {
      params: crud1.query
    })
    .then((req) => {
      statusTabCount.allCount = req.data.data.allCount;
      statusTabCount.finishedCount = req.data.data.finishedCount;
      statusTabCount.holdingCount = req.data.data.holdingCount;
      statusTabCount.rejectedCount = req.data.data.rejectedCount;
      statusTabCount.waitSubmitCount = req.data.data.waitSubmitCount;
    })
    .catch((err) => {
      crud1.loading = false;
    });
};
onBeforeMount(() => {
  crud1.toQuery();
  loadTabCountOfTable1();
});
onMounted(() => {
  // 表头拖拽必须在这里执行
  tableDrag(table1);
});
//tab切换事件
const tabActiveClick = (tab: TabsPaneContext, event: Event) => {

};
//高级查询条件
const queryList = computed(() => {
  return [
    {
      key: 'billCode1',
      label: '单号',
      show: true,
      props: {
        style: {
          width: '400px'
        }
      }
    },
    {
      key: 'createdTime',
      endDate: 'createdTimeEnd',
      type: 'datetimerange',
      label: '时间段',
      props: {
        'disabled-date': (date) => {
          return date.getTime() > Date.now();
        }
      },
      show: true,
      prepend: {
        props: {
          style: {
            width: '70px'
          },
          'disabled-date': (date) => {
            return date.getTime() > Date.now();
          }
        },
        key: 'agentProperty',
        label: '是否包含',
        type: 'select',
        labelK: 'name',
        valueK: 'id',
        dataList: [
          { id: '1', name: '包含' },
          { id: '2', name: '不包含' }
        ]
      }
    },
  ]
});
//表格1权限配置
const crudPermission = ref({
  add: ['/paymentAuto/All'],
  edit: ['/paymentAuto/All'],
  del: ['/paymentAuto/All'],
  download: ['/paymentAuto/All']
});

// Dialog success handler
const handleDialogSuccess = () => {
  crud1.toQuery();
};

//------------表格二（供应商信息）
const table2 = ref<InstanceType<typeof ElTable>>();
const crud2 = CRUD(
  {
    title: '供应商信息',
    url: '',
    idField: 'id',
    sort: [],
    query: {},
    defaultForm: () => {
      return {
        id: ''
      };
    },
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
  {
    table: table2
  }
);
</script>
