<template>
  <el-dialog
    v-model="visible"
    :title="isEdit ? '编辑批量付款单' : '新增批量付款单'"
    width="800px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="120px"
      class="crud-form"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="批量付款单号" prop="code">
            <el-input
              v-model="form.code"
              placeholder="请输入批量付款单号"
              :disabled="isEdit"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
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
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="公司" prop="companyName">
            <el-input
              v-model="form.companyName"
              placeholder="请输入公司名称"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="总金额" prop="sumValue">
            <el-input-number
              v-model="form.sumValue"
              :precision="2"
              :min="0"
              placeholder="请输入总金额"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="预计付款日期" prop="probablyPayTime">
            <el-date-picker
              v-model="form.probablyPayTime"
              type="date"
              placeholder="选择预计付款日期"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="状态" prop="status">
            <el-select
              v-model="form.status"
              placeholder="请选择状态"
              style="width: 100%"
            >
              <el-option label="待提交" value="0" />
              <el-option label="审批中" value="1" />
              <el-option label="已完成" value="2" />
              <el-option label="已驳回" value="11" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="备注" prop="remark">
        <el-input
          v-model="form.remark"
          type="textarea"
          :rows="3"
          placeholder="请输入备注"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        {{ isEdit ? '保存' : '创建' }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script lang="ts" setup>
import { ref, reactive, watch } from 'vue';
import { ElMessage, FormInstance, FormRules } from 'element-plus';

interface FormData {
  id?: string;
  code: string;
  billDate: string;
  companyName: string;
  sumValue: number;
  status: string;
  probablyPayTime: string;
  remark: string;
}

const props = defineProps<{
  visible: boolean;
  isEdit: boolean;
  data?: any;
}>();

const emit = defineEmits<{
  'update:visible': [value: boolean];
  success: [];
}>();

const formRef = ref<FormInstance>();
const submitting = ref(false);

const form = reactive<FormData>({
  code: '',
  billDate: '',
  companyName: '',
  sumValue: 0,
  status: '0',
  probablyPayTime: '',
  remark: ''
});

const rules: FormRules = {
  code: [{ required: true, message: '请输入批量付款单号', trigger: 'blur' }],
  billDate: [{ required: true, message: '请选择单据日期', trigger: 'change' }],
  companyName: [{ required: true, message: '请输入公司名称', trigger: 'blur' }],
  sumValue: [{ required: true, message: '请输入总金额', trigger: 'blur' }],
  status: [{ required: true, message: '请选择状态', trigger: 'change' }]
};

const visible = ref(false);

watch(() => props.visible, (newVal) => {
  visible.value = newVal;
  if (newVal) {
    resetForm();
    if (props.isEdit && props.data) {
      Object.assign(form, {
        id: props.data.id,
        code: props.data.code,
        billDate: props.data.billDate?.split('T')[0] || '',
        companyName: props.data.companyName,
        sumValue: props.data.sumValue || 0,
        status: props.data.status?.toString() || '0',
        probablyPayTime: props.data.probablyPayTime?.split('T')[0] || '',
        remark: props.data.remark || ''
      });
    } else {
      // 新增时设置默认日期
      form.billDate = new Date().toISOString().split('T')[0];
    }
  }
});

const resetForm = () => {
  Object.assign(form, {
    code: '',
    billDate: '',
    companyName: '',
    sumValue: 0,
    status: '0',
    probablyPayTime: '',
    remark: ''
  });
  formRef.value?.clearValidate();
};

const handleClose = () => {
  emit('update:visible', false);
};

const handleSubmit = async () => {
  if (!formRef.value) return;

  try {
    await formRef.value.validate();
    
    submitting.value = true;
    
    // 这里应该调用API，暂时模拟
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    ElMessage.success(props.isEdit ? '更新成功' : '创建成功');
    emit('success');
    handleClose();
  } catch (error) {
    console.error('表单验证失败:', error);
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.crud-form {
  padding: 20px 0;
}
</style>
