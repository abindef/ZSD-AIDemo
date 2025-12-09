<template>
  <div class="payment-auto-form">
    <el-form ref="formRef" :model="form" :rules="rules" label-width="120px">
      <!-- 基本信息 -->
      <el-card class="form-card">
        <template #header>
          <span>基本信息</span>
        </template>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="单号" prop="code">
              <el-input v-model="form.code" placeholder="请输入单号" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="单据日期" prop="billDate">
              <el-date-picker
                v-model="form.billDate"
                type="date"
                placeholder="选择日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="公司" prop="companyName">
              <el-input v-model="form.companyName" placeholder="请输入公司名称" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="公司编码" prop="companyCode">
              <el-input v-model="form.companyCode" placeholder="请输入公司编码" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="核算部门">
              <el-input v-model="form.departmentNamePath" placeholder="请输入核算部门" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="整体附言">
              <el-input v-model="form.overallPostscript" placeholder="请输入整体附言" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input
                v-model="form.remark"
                type="textarea"
                :rows="2"
                placeholder="请输入备注"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>

      <!-- 付款明细 -->
      <el-card class="form-card">
        <template #header>
          <div class="card-header">
            <span>付款明细</span>
            <el-button type="primary" size="small" @click="addDetail">
              <el-icon><Plus /></el-icon>
              添加明细
            </el-button>
          </div>
        </template>
        <el-table :data="form.details" border stripe size="small">
          <el-table-column type="index" label="序号" width="60" />
          <el-table-column label="付款单号" min-width="150">
            <template #default="scope">
              <el-input v-model="scope.row.paymentCode" placeholder="付款单号" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="付款金额" min-width="150">
            <template #default="scope">
              <el-input-number
                v-model="scope.row.paymentAmount"
                :precision="2"
                :min="0.01"
                placeholder="付款金额"
                size="small"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column label="付款时间" min-width="180">
            <template #default="scope">
              <el-date-picker
                v-model="scope.row.paymentTime"
                type="datetime"
                placeholder="付款时间"
                value-format="YYYY-MM-DD HH:mm:ss"
                size="small"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column label="折扣金额" min-width="150">
            <template #default="scope">
              <el-input-number
                v-model="scope.row.discountAmount"
                :precision="2"
                :min="0"
                placeholder="折扣金额"
                size="small"
                style="width: 100%"
              />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="scope">
              <el-button type="danger" link size="small" @click="removeDetail(scope.$index)">
                删除
              </el-button>
            </template>
          </el-table-column>
        </el-table>
        <div class="detail-summary">
          <span>合计金额：</span>
          <span class="amount">{{ formatMoney(totalDetailAmount) }}</span>
        </div>
      </el-card>

      <!-- 供应商信息 -->
      <el-card class="form-card">
        <template #header>
          <div class="card-header">
            <span>供应商信息</span>
            <el-button type="primary" size="small" @click="addAgent">
              <el-icon><Plus /></el-icon>
              添加供应商
            </el-button>
          </div>
        </template>
        <el-table :data="form.agents" border stripe size="small">
          <el-table-column type="index" label="序号" width="60" />
          <el-table-column label="供应商名称" min-width="150">
            <template #default="scope">
              <el-input v-model="scope.row.agentName" placeholder="供应商名称" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="账户名称" min-width="150">
            <template #default="scope">
              <el-input v-model="scope.row.accountName" placeholder="账户名称" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="账号" min-width="180">
            <template #default="scope">
              <el-input v-model="scope.row.accountNumber" placeholder="账号" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="开户行" min-width="180">
            <template #default="scope">
              <el-input v-model="scope.row.bankName" placeholder="开户行" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="支付类型" min-width="120">
            <template #default="scope">
              <el-select v-model="scope.row.paymentType" placeholder="支付类型" size="small" style="width: 100%">
                <el-option label="银行转账" value="bank" />
                <el-option label="支票" value="check" />
                <el-option label="现金" value="cash" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="转账附言" min-width="150">
            <template #default="scope">
              <el-input v-model="scope.row.transferRemark" placeholder="转账附言" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="scope">
              <el-button type="danger" link size="small" @click="removeAgent(scope.$index)">
                删除
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-card>
    </el-form>

    <!-- 底部按钮 -->
    <div class="form-footer">
      <el-button @click="handleCancel">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        {{ isEdit ? '保存' : '创建' }}
      </el-button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted, PropType } from 'vue';
import { ElMessage, FormInstance, FormRules } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import {
  createPaymentAuto,
  updatePaymentAuto,
  PaymentAutoItemDto,
  PaymentAutoDetailDto,
  PaymentAutoAgentDto,
  CreatePaymentAutoRequest,
  UpdatePaymentAutoRequest
} from '@/api/paymentAuto';

const props = defineProps({
  data: {
    type: Object as PropType<PaymentAutoItemDto | null>,
    default: null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['success', 'cancel']);

const formRef = ref<FormInstance>();
const submitting = ref(false);

// 表单数据
const form = reactive({
  id: '',
  code: '',
  billDate: '',
  companyId: '',
  companyName: '',
  companyCode: '',
  remark: '',
  overallPostscript: '',
  departmentIdPath: '',
  departmentNamePath: '',
  currentDepartmentId: '',
  attachmentIds: '',
  details: [] as PaymentAutoDetailDto[],
  agents: [] as PaymentAutoAgentDto[]
});

// 表单验证规则
const rules: FormRules = {
  code: [{ required: true, message: '请输入单号', trigger: 'blur' }],
  billDate: [{ required: true, message: '请选择单据日期', trigger: 'change' }],
  companyName: [{ required: true, message: '请输入公司名称', trigger: 'blur' }],
  companyCode: [{ required: true, message: '请输入公司编码', trigger: 'blur' }]
};

// 计算明细总金额
const totalDetailAmount = computed(() => {
  return form.details.reduce((sum, item) => sum + (item.paymentAmount || 0), 0);
});

// 格式化金额
const formatMoney = (value: number) => {
  if (value === null || value === undefined) return '0.00';
  return value.toLocaleString('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
};

// 添加明细
const addDetail = () => {
  form.details.push({
    debtDetailId: crypto.randomUUID(),
    paymentCode: '',
    paymentAmount: 0,
    paymentTime: undefined,
    discountAmount: undefined
  });
};

// 删除明细
const removeDetail = (index: number) => {
  form.details.splice(index, 1);
};

// 添加供应商
const addAgent = () => {
  form.agents.push({
    agentId: undefined,
    agentName: '',
    accountName: '',
    accountNumber: '',
    bankName: '',
    bankCode: '',
    paymentType: 'bank',
    transferRemark: ''
  });
};

// 删除供应商
const removeAgent = (index: number) => {
  form.agents.splice(index, 1);
};

// 取消
const handleCancel = () => {
  emit('cancel');
};

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    await formRef.value.validate();

    // 验证明细
    if (form.details.length === 0) {
      ElMessage.warning('请至少添加一条付款明细');
      return;
    }

    const invalidDetail = form.details.find((d) => !d.paymentAmount || d.paymentAmount <= 0);
    if (invalidDetail) {
      ElMessage.warning('付款金额必须大于0');
      return;
    }

    submitting.value = true;

    if (props.isEdit) {
      // 更新
      const request: UpdatePaymentAutoRequest = {
        id: form.id,
        code: form.code,
        billDate: form.billDate,
        companyId: form.companyId || undefined,
        companyName: form.companyName,
        companyCode: form.companyCode,
        remark: form.remark || undefined,
        overallPostscript: form.overallPostscript || undefined,
        departmentIdPath: form.departmentIdPath || undefined,
        departmentNamePath: form.departmentNamePath || undefined,
        currentDepartmentId: form.currentDepartmentId || undefined,
        attachmentIds: form.attachmentIds || undefined,
        details: form.details.map((d) => ({
          id: d.id,
          debtDetailId: d.debtDetailId,
          paymentCode: d.paymentCode,
          paymentAmount: d.paymentAmount,
          paymentTime: d.paymentTime,
          discountAmount: d.discountAmount
        })),
        agents: form.agents.map((a) => ({
          id: a.id,
          agentId: a.agentId,
          agentName: a.agentName,
          accountName: a.accountName,
          accountNumber: a.accountNumber,
          bankName: a.bankName,
          bankCode: a.bankCode,
          paymentType: a.paymentType,
          transferRemark: a.transferRemark,
          invoiceNo: a.invoiceNo,
          contractNo: a.contractNo,
          crossBorderPayment: a.crossBorderPayment,
          transactionCode: a.transactionCode,
          transactionRemark: a.transactionRemark,
          isBondedGoods: a.isBondedGoods,
          feeBearer: a.feeBearer,
          customsGoods: a.customsGoods,
          attachmentIds: a.attachmentIds,
          isDebtTransfer: a.isDebtTransfer,
          debtTransferAgentId: a.debtTransferAgentId,
          debtTransferAgentName: a.debtTransferAgentName,
          debtTransferAccount: a.debtTransferAccount,
          debtTransferBankAccount: a.debtTransferBankAccount,
          debtTransferBank: a.debtTransferBank,
          debtTransferBankName: a.debtTransferBankName,
          debtTransferBankCode: a.debtTransferBankCode,
          debtTransferAttachment: a.debtTransferAttachment,
          groupPaymentId: a.groupPaymentId
        }))
      };

      await updatePaymentAuto(form.id, request);
      ElMessage.success('更新成功');
    } else {
      // 创建
      const request: CreatePaymentAutoRequest = {
        code: form.code,
        billDate: form.billDate,
        companyId: form.companyId || undefined,
        companyName: form.companyName,
        companyCode: form.companyCode,
        remark: form.remark || undefined,
        overallPostscript: form.overallPostscript || undefined,
        departmentIdPath: form.departmentIdPath || undefined,
        departmentNamePath: form.departmentNamePath || undefined,
        currentDepartmentId: form.currentDepartmentId || undefined,
        attachmentIds: form.attachmentIds || undefined,
        details: form.details.map((d) => ({
          debtDetailId: d.debtDetailId,
          paymentCode: d.paymentCode,
          paymentAmount: d.paymentAmount,
          paymentTime: d.paymentTime,
          discountAmount: d.discountAmount
        })),
        agents: form.agents.map((a) => ({
          agentId: a.agentId,
          agentName: a.agentName,
          accountName: a.accountName,
          accountNumber: a.accountNumber,
          bankName: a.bankName,
          bankCode: a.bankCode,
          paymentType: a.paymentType,
          transferRemark: a.transferRemark
        }))
      };

      await createPaymentAuto(request);
      ElMessage.success('创建成功');
    }

    emit('success');
  } catch (error: any) {
    if (error !== 'cancel' && error?.message) {
      ElMessage.error(error.message || '操作失败');
    }
  } finally {
    submitting.value = false;
  }
};

// 初始化数据
onMounted(() => {
  if (props.data && props.isEdit) {
    Object.assign(form, {
      id: props.data.id,
      code: props.data.code,
      billDate: props.data.billDate?.split('T')[0] || '',
      companyId: props.data.companyId || '',
      companyName: props.data.companyName,
      companyCode: props.data.companyCode,
      remark: props.data.remark || '',
      overallPostscript: props.data.overallPostscript || '',
      departmentIdPath: props.data.departmentIdPath || '',
      departmentNamePath: props.data.departmentNamePath || '',
      currentDepartmentId: props.data.currentDepartmentId || '',
      attachmentIds: props.data.attachmentIds || '',
      details: props.data.details?.map((d) => ({ ...d })) || [],
      agents: props.data.agents?.map((a) => ({ ...a })) || []
    });
  } else {
    // 新增时设置默认值
    form.billDate = new Date().toISOString().split('T')[0];
  }
});
</script>

<style scoped>
.payment-auto-form {
  max-height: 70vh;
  overflow-y: auto;
}

.form-card {
  margin-bottom: 16px;
}

.form-card:last-of-type {
  margin-bottom: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.detail-summary {
  margin-top: 12px;
  text-align: right;
  font-size: 14px;
}

.detail-summary .amount {
  font-size: 18px;
  font-weight: bold;
  color: #409eff;
}

.form-footer {
  margin-top: 20px;
  padding-top: 16px;
  border-top: 1px solid #e4e7ed;
  text-align: right;
}
</style>
