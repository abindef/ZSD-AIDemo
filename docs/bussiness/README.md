# 批量付款功能文档索引

## 文档结构

本目录包含批量付款功能的完整技术文档，按以下结构组织：

### 核心文档

| 文档 | 说明 |
|------|------|
| [14.用户故事.md](./14.用户故事.md) | **主文档** - 包含完整的用户故事、数据模型、业务规则、API设计、UI交互说明 |
| [11.批量付款数据库设计.md](./11.批量付款数据库设计.md) | 数据库表结构设计（原始设计，供参考） |
| [12.批量付款管理测试用例文档.md](./12.批量付款管理测试用例文档.md) | 测试用例文档 |

### 参考文档

| 文档 | 说明 |
|------|------|
| [01.项目结构.md](./01.项目结构.md) | 项目整体结构说明 |
| [02.财务业务知识库.md](./02.财务业务知识库.md) | 财务业务知识库 |
| [06.统一术语表.md](./06.统一术语表.md) | 统一术语定义 |

### 已归档文档

以下文档内容已整合到 `14.用户故事.md`，保留供历史参考：

- `13.批量付款前后端接口交互设计.md` - API设计（已整合）
- `15.批量付款新增功能详细设计.md` - 新增功能设计（已整合）
- `16.前端页面.md` - 前端页面结构（已整合）

### 实现总结文档（历史记录）

以下为开发过程中的实现总结，供历史追溯：

- `批量付款实现总结.md`
- `批量付款前端实现说明.md`
- `供应商和应付表实现总结.md`
- `数据库迁移完成报告.md`
- `数据初始化优化总结.md`

---

## 本次对话重要注意事项

### 1. 数据库配置

**开发环境使用内存数据库**：
```json
// appsettings.Development.json
{
  "UseInMemoryDatabase": true
}
```

如需使用 SQL Server，设置为 `false` 并配置连接字符串。

### 2. API 路由

所有 API 路由已统一为 `/api/` 前缀：

```
GET  /api/paymentAuto/page          # 分页查询
GET  /api/paymentAuto/statusCount   # 状态统计
GET  /api/paymentAuto/{id}          # 获取详情
GET  /api/paymentAuto/{id}/agents   # 获取供应商
GET  /api/paymentAuto/{id}/details  # 获取明细
POST /api/paymentAuto               # 创建
PUT  /api/paymentAuto/{id}          # 更新
DELETE /api/paymentAuto/{ids}       # 删除
POST /api/paymentAuto/{id}/submit   # 提交审批
GET  /api/debt/details              # 查询应付明细
```

### 3. 前端对话框触发方式

**重要**：使用 `inno-mc-vue3` 的 CRUD 组件时，新增/编辑对话框必须通过 `hooks` 触发：

```typescript
// ❌ 错误方式 - 不会触发对话框
crudMethod: {
  add: () => { dialogVisible.value = true }
}

// ✅ 正确方式 - 使用 hooks
hooks: {
  [CRUD.HOOK.afterToAdd]: (_crud: any) => {
    dialogVisible.value = true
  },
  [CRUD.HOOK.afterToEdit]: (_crud: any) => {
    dialogData.value = _crud.form
    dialogVisible.value = true
  }
}
```

### 4. Result 类方法命名

Application 层的 `Result` 类使用以下方法：

```csharp
// 成功
Result.Ok()                    // 无返回值
Result.Ok<T>(value)            // 有返回值

// 失败
Result.Fail(message)           // 无返回值
Result.Fail<T>(message)        // 有返回值
```

### 5. 公司和部门选择

新增/编辑对话框中：
- **公司**：必填，下拉选择，从后端接口返回
- **核算部门**：必填，下拉选择，从后端接口返回

选择公司和部门后，才能查询应付明细列表。

---

## 技术栈

### 后端
- .NET 8.0
- ASP.NET Core 8.0
- Entity Framework Core 8.0 (Code First)
- DDD 分层架构

### 前端
- Vue 3 (Composition API + `<script setup>`)
- TypeScript
- @inno/inno-mc-vue3 (企业内部组件库)
- Element Plus

---

## 快速开始

### 启动后端

```bash
cd Finance/src/Inno.CorePlatform.Finance.Backend
dotnet run
```

后端服务地址：
- HTTP: http://localhost:61803
- HTTPS: https://localhost:61801

### 启动前端

```bash
cd Finance/src/Inno.CorePlatform.Finance.Frondtend
yarn dev
```

---

## 更新日志

| 日期 | 更新内容 |
|------|----------|
| 2025-12-15 | 初始化批量付款功能，完成前后端基础框架 |
| 2025-12-15 | 修复对话框触发问题，统一 API 路由前缀 |
| 2025-12-15 | 添加内存数据库支持，解决 SQL Server 连接问题 |
| 2025-12-15 | 整合文档，删除重复内容 |
