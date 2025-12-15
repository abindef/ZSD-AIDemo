<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '编辑批量付款单' : '新增批量付款单'"
    width="90%"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form ref="formRef" :model="formData" :rules="rules" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="公司" prop="companyId">
            <el-select
              v-model="formData.companyId"
              placeholder="请选择公司"
              filterable
              style="width: 100%"
              @change="handleCompanyChange"
            >
              <el-option
                v-for="item in companyList"
                :key="item.id"
                :label="item.name"
                :value="item.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="核算部门" prop="departmentIdPath">
            <el-cascader
              v-model="formData.departmentIdPath"
              :options="departmentTree"
              :props="departmentProps"
              placeholder="请选择核算部门"
              filterable
              style="width: 100%"
              @change="handleDepartmentChange"
            />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="备注">
            <el-input v-model="formData.remark" placeholder="请输入备注" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <el-divider />

    <!-- 应付单查询条件 -->
    <div style="margin-bottom: 16px;">
      <el-form :inline="true" :model="debtQuery">
        <el-form-item label="核算部门">
          <el-input v-model="debtQuery.departmentName" placeholder="请选择核算部门" disabled style="width: 200px" />
        </el-form-item>
        <el-form-item label="公司">
          <el-input v-model="debtQuery.companyName" placeholder="请选择公司" disabled style="width: 200px" />
        </el-form-item>
        <el-form-item label="客户">
          <el-input v-model="debtQuery.customer" placeholder="请输入客户" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="供应商">
          <el-select v-model="debtQuery.agentId" placeholder="请选择供应商" filterable clearable style="width: 200px">
            <el-option
              v-for="item in agentList"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="应付单号">
          <el-input v-model="debtQuery.code" placeholder="请输入应付单号" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="业务单元">
          <el-input v-model="debtQuery.businessUnit" placeholder="请输入业务单元" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="项目名称">
          <el-input v-model="debtQuery.projectName" placeholder="请输入项目名称" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="币种">
          <el-select v-model="debtQuery.currency" placeholder="请选择币种" clearable style="width: 120px">
            <el-option label="人民币" value="CNY" />
            <el-option label="美元" value="USD" />
            <el-option label="欧元" value="EUR" />
          </el-select>
        </el-form-item>
        <el-form-item label="采购合同号">
          <el-input v-model="debtQuery.purchaseContractNo" placeholder="请输入采购合同号" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadDebtDetailList">查询</el-button>
          <el-button @click="resetDebtQuery">清空</el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 单据类型Tab标签页 -->
    <el-tabs v-model="activeDocumentType" @tab-click="handleDocumentTypeChange">
      <el-tab-pane label="预付账期" name="prepayment" />
      <el-tab-pane label="入库账期" name="warehousing" />
      <el-tab-pane label="销售账期" name="sales" />
      <el-tab-pane label="回款账期" name="collection" />
      <el-tab-pane label="验收账期" name="acceptance" />
      <el-tab-pane label="质保账期" name="warranty" />
    </el-tabs>

    <!-- 应付明细列表 -->
    <el-table
      ref="debtTableRef"
      v-loading="debtLoading"
      :data="debtDetailList"
      border
      stripe
      max-height="350"
      @selection-change="handleDebtSelectionChange"
    >
      <el-table-column type="selection" width="55" :selectable="checkSelectable" />
      <el-table-column type="index" label="序号" width="60" />
      <el-table-column prop="debtCode" label="应付单号" min-width="160" show-overflow-tooltip />
      <el-table-column prop="agentName" label="供应商" min-width="160" show-overflow-tooltip />
      <el-table-column prop="businessUnit" label="业务单元" min-width="120" show-overflow-tooltip />
      <el-table-column prop="customer" label="客户" min-width="140" show-overflow-tooltip />
      <el-table-column prop="currency" label="币种" width="80" show-overflow-tooltip />
      <el-table-column prop="expectedPaymentDate" label="预计付款日期" min-width="120" show-overflow-tooltip>
        <template #default="scope">
          {{ dateFormat(scope.row.expectedPaymentDate, 'YYYY-MM-DD') }}
        </template>
      </el-table-column>
      <el-table-column prop="paymentTermType" label="付款条件" min-width="100" show-overflow-tooltip />
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
      <el-table-column prop="purchaseContractNo" label="采购合同号" min-width="140" show-overflow-tooltip />
      <el-table-column prop="projectName" label="项目名称" min-width="140" show-overflow-tooltip />
      <el-table-column label="本次付款金额" min-width="150" align="right" fixed="right">
        <template #default="scope">
          <el-input-number
            v-model="scope.row.currentPaymentAmount"
            :min="0"
            :max="scope.row.remainingAmount"
            :precision="2"
            :controls="false"
            style="width: 100%"
            @change="handlePaymentAmountChange"
          />
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <div style="margin-top: 16px; text-align: right;">
      <el-pagination
        v-model:current-page="pagination.page"
        v-model:page-size="pagination.limit"
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.total"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />
    </div>

    <!-- 汇总信息 -->
    <div style="margin-top: 16px; padding: 12px; background: #f5f7fa; border-radius: 4px;">
      <el-row :gutter="20">
        <el-col :span="8">
          <span style="font-weight: bold;">明细数量：</span>
          <span style="color: #409eff; font-size: 16px;">{{ selectedDebtDetails.length }}</span>
        </el-col>
        <el-col :span="8">
          <span style="font-weight: bold;">供应商数量：</span>
          <span style="color: #409eff; font-size: 16px;">{{ supplierCount }}</span>
        </el-col>
        <el-col :span="8">
          <span style="font-weight: bold;">总金额：</span>
          <span style="color: #f56c6c; font-size: 18px; font-weight: bold;">{{ formatAmount(totalAmount) }}</span>
        </el-col>
      </el-row>
    </div>

    <template #footer>
      <span class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">保存</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, watch, nextTick } from 'vue';
import { ElMessage, ElTable } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { dateFormat } from '@inno/inno-mc-vue3/lib/utils/filters';
import { paymentAutoApi, debtApi, companyApi, departmentApi, agentApi } from '@/api/paymentAuto';
import type { 
  PaymentAutoItem, 
  DebtDetail, 
  Company, 
  Department,
  Agent,
  PaymentAutoFormData 
} from '@/types/paymentAuto';

interface Props {
  visible: boolean;
  isEdit: boolean;
  data: PaymentAutoItem | null;
}

interface Emits {
  (e: 'update:visible', value: boolean): void;
  (e: 'success'): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const formRef = ref<FormInstance>();
const debtTableRef = ref<InstanceType<typeof ElTable>>();

const formData = reactive<PaymentAutoFormData>({
  companyId: '',
  companyName: '',
  departmentIdPath: '',
  departmentNamePath: '',
  departmentCurrentId: '',
  remark: '',
  selectedDebtDetails: []
});

const rules: FormRules = {
  companyId: [{ required: true, message: '请选择公司', trigger: 'change' }],
  departmentIdPath: [{ required: true, message: '请选择核算部门', trigger: 'change' }]
};

const submitLoading = ref(false);
const debtLoading = ref(false);

const companyList = ref<Company[]>([]);
const departmentTree = ref<Department[]>([]);
const agentList = ref<Agent[]>([]);
const debtDetailList = ref<DebtDetail[]>([]);
const selectedDebtDetails = ref<DebtDetail[]>([]);

const departmentProps = {
  value: 'id',
  label: 'name',
  children: 'children',
  checkStrictly: false,
  emitPath: true
};

const debtQuery = reactive({
  code: '',
  agentId: '',
  departmentName: '',
  companyName: '',
  customer: '',
  businessUnit: '',
  projectName: '',
  currency: '',
  purchaseContractNo: ''
});

const activeDocumentType = ref('prepayment');

const pagination = reactive({
  page: 1,
  limit: 20,
  total: 0
});

watch(() => props.visible, (val) => {
  if (val) {
    initDialog();
  } else {
    resetForm();
  }
});

const initDialog = async () => {
  await loadCompanyList();
  await loadDepartmentTree();
  await loadAgentList();

  if (props.isEdit && props.data) {
    Object.assign(formData, {
      id: props.data.id,
      companyId: props.data.companyId,
      companyName: props.data.companyName,
      departmentIdPath: props.data.departmentIdPath,
      departmentNamePath: props.data.departmentNamePath,
      departmentCurrentId: props.data.departmentCurrentId,
      remark: props.data.remark
    });
    
    await loadDebtDetailList();
    await loadExistingDetails();
  }
};

const loadCompanyList = async () => {
  try {
    const res = await companyApi.getList();
    if (res.data && res.data.data) {
      companyList.value = res.data.data;
    }
  } catch (err) {
    console.error('加载公司列表失败:', err);
  }
};

const loadDepartmentTree = async () => {
  try {
    const res = await departmentApi.getTree();
    if (res.data && res.data.data) {
      departmentTree.value = res.data.data;
    }
  } catch (err) {
    console.error('加载部门树失败:', err);
  }
};

const loadAgentList = async () => {
  try {
    const res = await agentApi.getList();
    if (res.data && res.data.data) {
      agentList.value = res.data.data;
    }
  } catch (err) {
    console.error('加载供应商列表失败:', err);
  }
};

const loadDebtDetailList = async () => {
  if (!formData.companyId || !formData.departmentIdPath) {
    ElMessage.warning('请先选择公司和核算部门');
    return;
  }

  debtLoading.value = true;
  try {
    const res = await debtApi.getDetailList({
      companyId: formData.companyId,
      departmentIdPath: formData.departmentIdPath,
      documentType: activeDocumentType.value,
      code: debtQuery.code,
      agentId: debtQuery.agentId,
      customer: debtQuery.customer,
      businessUnit: debtQuery.businessUnit,
      projectName: debtQuery.projectName,
      currency: debtQuery.currency,
      purchaseContractNo: debtQuery.purchaseContractNo,
      page: pagination.page,
      limit: pagination.limit
    });
    
    if (res.data && res.data.data) {
      debtDetailList.value = res.data.data.list?.map((item: DebtDetail) => ({
        ...item,
        currentPaymentAmount: 0
      })) || [];
      pagination.total = res.data.data.total || 0;
    }
  } catch (err) {
    console.error('加载应付明细失败:', err);
    debtDetailList.value = [];
    pagination.total = 0;
  } finally {
    debtLoading.value = false;
  }
};

const loadExistingDetails = async () => {
  if (!props.data?.id) return;

  try {
    const res = await paymentAutoApi.getDetails(props.data.id);
    if (res.data && res.data.data) {
      const existingDetails = res.data.data;
      
      await nextTick();
      
      existingDetails.forEach((detail: any) => {
        const debtDetail = debtDetailList.value.find(d => d.id === detail.debtDetailId);
        if (debtDetail) {
          debtDetail.currentPaymentAmount = detail.paymentAmount;
          debtTableRef.value?.toggleRowSelection(debtDetail, true);
        }
      });
    }
  } catch (err) {
    console.error('加载已有明细失败:', err);
  }
};

const handleCompanyChange = (companyId: string) => {
  const company = companyList.value.find(c => c.id === companyId);
  if (company) {
    formData.companyName = company.name;
    debtQuery.companyName = company.name;
  }
  
  debtDetailList.value = [];
  selectedDebtDetails.value = [];
};

const handleDepartmentChange = (value: string[]) => {
  if (value && value.length > 0) {
    formData.departmentCurrentId = value[value.length - 1];
    
    const findDepartment = (depts: Department[], path: string[]): Department | null => {
      for (const dept of depts) {
        if (dept.id === path[0]) {
          if (path.length === 1) {
            return dept;
          }
          if (dept.children) {
            return findDepartment(dept.children, path.slice(1));
          }
        }
      }
      return null;
    };
    
    const dept = findDepartment(departmentTree.value, value);
    if (dept) {
      formData.departmentNamePath = dept.namePath;
      debtQuery.departmentName = dept.namePath;
    }
  }
  
  debtDetailList.value = [];
  selectedDebtDetails.value = [];
};

const handleDebtSelectionChange = (selection: DebtDetail[]) => {
  selectedDebtDetails.value = selection;
};

const handlePaymentAmountChange = () => {
  // 触发汇总计算
};

const checkSelectable = (row: DebtDetail) => {
  return row.remainingAmount > 0;
};

const handleDocumentTypeChange = () => {
  pagination.page = 1;
  loadDebtDetailList();
};

const handleSizeChange = (size: number) => {
  pagination.limit = size;
  pagination.page = 1;
  loadDebtDetailList();
};

const handleCurrentChange = (page: number) => {
  pagination.page = page;
  loadDebtDetailList();
};

const resetDebtQuery = () => {
  debtQuery.code = '';
  debtQuery.agentId = '';
  debtQuery.customer = '';
  debtQuery.businessUnit = '';
  debtQuery.projectName = '';
  debtQuery.currency = '';
  debtQuery.purchaseContractNo = '';
  pagination.page = 1;
  loadDebtDetailList();
};

const supplierCount = computed(() => {
  const agentIds = new Set(selectedDebtDetails.value.map(d => d.agentName));
  return agentIds.size;
});

const totalAmount = computed(() => {
  return selectedDebtDetails.value.reduce((sum, detail) => {
    return sum + (detail.currentPaymentAmount || 0);
  }, 0);
});

const formatAmount = (amount: number | undefined | null): string => {
  if (amount === undefined || amount === null) return '0.00';
  return amount.toLocaleString('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

const handleSubmit = async () => {
  if (!formRef.value) return;

  await formRef.value.validate(async (valid) => {
    if (!valid) return;

    if (selectedDebtDetails.value.length === 0) {
      ElMessage.warning('请至少选择一条应付明细');
      return;
    }

    const hasInvalidAmount = selectedDebtDetails.value.some(
      detail => !detail.currentPaymentAmount || detail.currentPaymentAmount <= 0
    );
    if (hasInvalidAmount) {
      ElMessage.warning('请为所有选中的明细填写本次付款金额');
      return;
    }

    const hasExceedAmount = selectedDebtDetails.value.some(
      detail => detail.currentPaymentAmount! > detail.remainingAmount
    );
    if (hasExceedAmount) {
      ElMessage.warning('本次付款金额不能超过剩余金额');
      return;
    }

    submitLoading.value = true;
    try {
      const submitData = {
        ...formData,
        details: selectedDebtDetails.value.map(detail => ({
          debtDetailId: detail.id,
          paymentAmount: detail.currentPaymentAmount
        }))
      };

      if (props.isEdit && props.data?.id) {
        await paymentAutoApi.update(props.data.id, submitData);
        ElMessage.success('更新成功');
      } else {
        await paymentAutoApi.create(submitData);
        ElMessage.success('创建成功');
      }

      emit('success');
      handleClose();
    } catch (err: any) {
      ElMessage.error(err.message || '操作失败');
    } finally {
      submitLoading.value = false;
    }
  });
};

const handleClose = () => {
  emit('update:visible', false);
};

const resetForm = () => {
  formRef.value?.resetFields();
  Object.assign(formData, {
    companyId: '',
    companyName: '',
    departmentIdPath: '',
    departmentNamePath: '',
    departmentCurrentId: '',
    remark: '',
    selectedDebtDetails: []
  });
  debtDetailList.value = [];
  selectedDebtDetails.value = [];
  Object.assign(debtQuery, {
    code: '',
    agentId: '',
    departmentName: '',
    companyName: '',
    customer: '',
    businessUnit: '',
    projectName: '',
    currency: '',
    purchaseContractNo: ''
  });
  activeDocumentType.value = 'prepayment';
  Object.assign(pagination, {
    page: 1,
    limit: 20,
    total: 0
  });
};
</script>

<style scoped>
:deep(.el-dialog__body) {
  padding: 20px;
}

:deep(.el-input-number .el-input__inner) {
  text-align: right;
}
</style>
