# Frontend-Of-AI

## 一、项目基本信息

- **项目名称**：frontend-of-ai
- **保存地址**：`D:\AI\Frondtend`  
- **参考结构来源**：`D:\致新\致新-finance-management\src\Inno.CorePlatform.Finance.Frontend`

---

## 二、项目结构

> 说明：本项目整体结构参照  
> `D:\致新\致新-finance-management\src\Inno.CorePlatform.Finance.Frontend`  
> 后续可根据实际目录（如 `src/`, `public/`, `tests/` 等）在此处补充详细树形结构与说明。

示例（可根据实际情况调整）：

- `public/`：静态资源（favicon、公共 JS 等）
- `src/`
  - `api/`：接口封装
  - `assets/`：静态资源（图片、样式等）
  - `component/`：通用组件
  - `router/`：路由配置
  - `...`
- `tests/`
  - `unit/`：单元测试（Jest）
  - `e2e/`：端到端测试（Cypress）
- `types/`：TS 类型声明
- 其他配置文件：`vite.config.ts`、`tsconfig.json`、`eslint.config.js` 等

---

## 三、技术栈

### 3.1 基础框架

- **框架**：Vue 3  
- **语言**：TypeScript  
- **构建工具**：Vite（底层 Rollup）  
- **包管理**：
  - NPM
  - Yarn  
  - 实际以 `package-lock.json` 为准（以锁文件确定包管理器及版本）

---

## 四、代码质量与规范

### 4.1 代码质量工具

- **ESLint**：语法与编码规范检查  
- **Stylelint**：样式规范检查  
- **Prettier**：代码格式化  
- **lint-staged**：提交前针对变更文件执行 lint/format

> 建议在 `pre-commit` 中统一接入上述工具，保证提交代码前即完成基础检查与自动修复。

---

## 五、测试体系

- **Jest**：单元测试框架  
- **Cypress**：端到端（E2E）测试

> 可针对核心业务流程（如单据创建、审批流、统计查询等）编写 E2E 场景用例。

---

## 六、UI 与组件库

- **Element Plus 图标**
- **公司内部 Vue3 组件库**  
  - 包名：`inno/inno-mc-vue3`

> 页面搭建优先复用公司内部组件库，其次补充 Element Plus 相关组件与图标。

---

## 七、国际化（i18n）

- **方案**：`intlify/*` 系列（vue-i18n 生态）

> 建议：
> - 按业务模块拆分多语言资源文件
> - 统一文案 key 命名规范（如：`finance.invoice.title`）

---

## 八、其他能力

- **环境变量配置**  
  - 支持多环境 `.env` 文件（如：`.env.development`、`.env.production` 等）
  - 通过 Vite 环境变量机制进行注入和区分

- **JSON Schema 校验**  
  - 使用 **Ajv** 进行 JSON Schema 校验
  - 可用于表单数据、接口返回数据结构校验

- **代码覆盖率**  
  - 使用 **Istanbul** 相关工具链统计代码覆盖率
  - 一般结合 Jest/Cypress 测试结果生成覆盖率报告

---

## 九、后续规划（可选）

> 以下为预留小节，可根据项目推进情况补充：

- **需求背景与业务范围**
- **主要功能模块说明**
- **接口规范与约定**
- **部署与发布流程**
- **日志与监控方案**