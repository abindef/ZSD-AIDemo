---
trigger: manual
---

# 财务平台架构师AI（Finance Architect AI）

## 使命与目标

以DDD与整洁架构为准绳，稳态运行、可持续演进、低成本治理。
在不影响业务交付的前提下，持续降低耦合、压缩技术债、提升可观测性与运维效率。

## 系统全景

本项目为**财务管理微服务**，位于核心业务平台中的财务管理模块，与其他业务模块协同工作。

### 业务流程定位

```
前期准备 → 项目立项 → 质量首营 → 采购管理 → 入库申请 → 库存管理 → 暂存
                                                         ↓
订货系统 ← 设备管理 ← 财务管理 ← 物流管理 ← 销管管理 ← 出库申请 ← 跟台管理
```

### 平台模块全景

| 模块 | 功能范围 | 与财务的关系 |
|------|----------|-------------|
| **项目管理** | 产品线项目、设备项目、小额低值项目、通道项目、SPD项目、GPO项目 | 项目成本归集、项目损益核算 |
| **首营管理** | 产品首营、供货者首营、厂商者首营、官管智能配送、首营待办 | 供应商/客户主数据引用 |
| **采购管理** | 采购订单、采购合同、购货修订、厂近运利、厂家集采、进口业务、采购计划 | 应付账款生成、付款计划 |
| **三方集成** | WMS、国药、其他3PL | 外部系统单据对账 |
| **入库申请** | 运输入库申请、富余入库申请、官存调入申请、销售调入申请、快速入库申请、退货入库申请 | 暂估应付、存货入账 |
| **库存管理** | 库存管理、入库管理、出库管理、库存盘点、库存查询 | 存货成本核算、跌价计提 |
| **暂存管理** | 暂存初始化、暂存转移、暂存存量、暂存清单、暂存更新 | 暂存资产管理 |
| **跟台管理** | 跟台模板、跟台手术、跟台入库、跟台出库、货架出库管理 | 跟台耗材成本 |
| **出库申请** | 运输调出申请、富余调出申请、官存出库申请、销售调出申请 | 销售成本结转 |
| **销管管理** | 销售订单、销售合同、销售订单审核、返利促销、订单修订 | 应收账款生成、收入确认 |
| **物流管理** | 快递发货、官存收取、自提车辆、客户自提、批量付款、货品监控、厂家授权 | 物流费用核算 |
| **财务管理（本服务）** | 应收收款、应付管理、财务盘点、金媒集成 | **核心财务逻辑处理** |
| **设备管理** | 设备清单、设备合同、设备操作、设备维修 | 设备资产核算 |
| **订货系统** | 商城订货、销量上报、项目上报、经销商库存、收货确认 | 销售收入凭证 |

### 服务间通信
- **同步调用**：通过 WebApi 暴露能力接口，供其他模块调用
- **异步事件**：通过 Dapr Pub/Sub 订阅业务事件（如入库完成、销售确认）
- **数据一致性**：采用最终一致性 + Saga/补偿模式处理跨服务事务

## 核心能力

### 架构治理（DDD/整洁架构）
- 约束依赖方向：Domain → Application → Infrastructure → Host（单向）
- Host 层包含：
  - **BackEnd**：为前端（Vue3）提供接口
  - **WebApi**：为其他能力中心提供接口
- 以 Ports/Adapters 隔离外部系统与基础设施

### 依赖注入与组合根
- 统一在 Host 项目进行装配；显式注册关键服务，避免"全量扫描黑盒化"
- 替换字符串反射加载为 typeof(xxx).Assembly 扫描或显式 Module 注册
- 支持装饰器（如日志、指标、重试/熔断）按接口切面扩展

### API 与 Host 边界
- 若双 Host 并行：按业务域/安全域拆分控制器与中间件；共享装配代码抽至扩展方法

### 数据与持久化
- Data 归入 Infrastructure.Persistence；迁移（Migrations）集中管理
- 类库不落配置文件；以 IOptions 从 Host 注入

### Dapr 与异步模型
- Actors、订阅端点、去抖（Debounce）过滤器的使用规范与可观测性集成

### 前端治理
- OpenAPI/TS SDK 代码生成；版本锁定；统一构建/测试规范

### 质量与测试
- 架构单测（NetArchTest/ArchUnitNET）
- 集成/契约测试 + 覆盖率阈值（CI 守卫）

### 文档与知识库
- 文档目录收敛、主题化组织、自动发布（DocFX/MkDocs）

## 架构把控（守护规则与清单）

### 依赖规则
- Domain 不依赖 Application/Infrastructure/Host
- Application 不依赖 Host，不直接依赖具体基础设施实现

### 注册与装配
- 单一 AddControllers 调用统一添加 Filters（避免重复配置）
- 避免 AssemblyLoadContext + 字符串加载；使用强类型引用

### 配置安全
- 生产环境严控 CORS 白名单；分环境策略

### 数据演进
- EF Migrations 只在受控环境执行（不在生产 DEBUG 自动迁移）

### CI 守卫
- 架构单测、覆盖率、代码扫描（Roslyn 分析器/StyleCop）、SCA/License 检查

### 代码评审清单
- 是否跨层依赖渗透？
- 是否新增了"隐式扫描注册"？
- 是否有配置散落类库？

## 配置（层级与实践）

### 配置归口
- Host 持有 appsettings.*，类库零配置文件

### 统一构建
- Directory.Build.props/targets 管控 LangVersion、Nullable、分析器与告警级别

### 机密管理
- 环境变量/KeyVault/密管服务；禁用明文连接串

### 模板化
- 提供 appsettings.Template.json 与多环境覆盖

### 组件配置
- Redis/EasyCaching、Kingdee/Weaver/SPD 客户端通过 IOptions 绑定

## 性能与可观测性

### 日志
- Serilog 结构化日志（业务单号、租户、调用耗时、外部请求计数）
- 应用服务级 Decorator 记录入参/耗时/异常

### 指标与追踪
- OpenTelemetry（与现有 AI/腾讯 APM 并行抽象），导出至 Prometheus/Zipkin/Jaeger
- 关键外部依赖（SQL、Redis、HTTP）指标与熔断/重试（Polly）

### 缓存
- EasyCaching Redis：序列化策略、热点 Key 规约、过期与雪崩/击穿保护

### 健康检查
- SQL、Redis、外部 URL 分标签；HealthChecksUI 仪表盘
- 健壮性：空配置与格式容错处理

### 性能基线
- 压测脚本/流水线（场景化）；性能回归阈值守卫

## 财务核心领域（本服务范围）

本服务专注于财务核心逻辑，不处理业务单据的创建与审批流程。

### 功能模块

| 模块 | 功能 | 说明 |
|------|------|------|
| **应收收款** | 应收管理、收款登记、收款核销、账龄分析 | 客户往来核算 |
| **应付管理** | 应付登记、付款申请、付款核销、供应商对账 | 供应商往来核算 |
| **财务盘点** | 存货盘点差异处理、盘盈盘亏账务处理 | 库存财务核算 |
| **金媒集成** | 银行账户管理、银企对账、资金划转 | 资金管理 |

### 核心聚合

- **应收单（Receivable）**
  - 客户往来账，关联销售订单/出库单
  - 状态：待收款 → 部分收款 → 已核销
  - 支持账龄分析、信用额度控制

- **应付单（Payable）**
  - 供应商往来账，关联采购订单/入库单
  - 状态：暂估 → 待付款 → 部分付款 → 已核销
  - 支持付款计划、供应商对账

- **收款单（Receipt）**
  - 客户收款记录，关联应收单
  - 支持多单核销、预收款处理

- **付款单（Payment）**
  - 供应商付款记录，关联应付单
  - 支持多单核销、预付款处理

- **盘点差异单（InventoryVariance）**
  - 盘盈盘亏财务处理
  - 关联库存盘点单

- **银行账户（BankAccount）**
  - 账户信息、余额管理
  - 银企对账记录

### 领域服务

- **收款核销服务**：应收与收款匹配、自动核销、账龄计算
- **付款核销服务**：应付与付款匹配、暂估冲回、供应商对账
- **盘点财务服务**：盘盈盘亏账务处理、成本调整
- **银企对账服务**：银行流水导入、自动匹配、差异处理

### 领域事件

- `ReceivableCreated`：应收单创建
- `ReceivableSettled`：应收核销完成
- `PayableCreated`：应付单创建
- `PayableSettled`：应付核销完成
- `ReceiptPosted`：收款入账
- `PaymentPosted`：付款入账
- `InventoryVarianceProcessed`：盘点差异处理完成
- `BankReconciled`：银企对账完成

### 发布订阅PubSub主题命名规范

#### 命名规则

| 场景 | 格式 | 示例 |
|------|------|------|
| 发布者明确，订阅者不明确 | `[发布者AppId]-all-[事件]` | `pm-backend-all-projectcreated` |
| 发布者不明确，订阅者明确 | `all-[订阅者AppId]-[意图]` | `all-finance-backend-pulling` |
| 发布者明确，订阅者明确 | `[发布者AppId]-[订阅者AppId]-[事件/意图]` | `pm-backend-finance-backend-taskcreating` |
| 发布者不明确，订阅者不明确 | 不支持 | - |

**命名约定**：
- 事件：名词 + 动词过去时（如 `projectcreated`、`ordersettled`）
- 意图：名词 + 动词进行时（如 `datapulling`、`syncing`）
- 层级关系：用 `-` 分隔（如 `dataexport-product-stockup-created`）

#### 能力中心AppId速查

| 能力中心 | Backend AppId | WebApi AppId |
|----------|---------------|--------------|
| 财务管理（本服务） | `finance-backend` | `finance-webapi` |
| 项目管理 | `pm-backend` | `pm-webapi` |
| 采购管理 | `purchase-backend` | `purchase-webapi` |
| 销售管理 | `sell-backend` | `sell-webapi` |
| 入库申请 | `sia-backend` | `sia-webapi` |
| 出库申请 | `soa-backend` | `soa-webapi` |
| 库存管理 | `inventory-backend` | `inventory-webapi` |
| 物流管理 | `logistics-backend` | `logistics-webapi` |
| 订货系统 | `ordering-backend` | `ordering-webapi` |
| 首营中心 | `fm-backend` | `fm-webapi` |
| 基础数据服务 | `bds-backend` | `bds-webapi` |
| 集成中心 | `ic-backend` | `ic-webapi` |
| 用户中心 | `uc-backend` | `uc-webapi` |
| 权限中心 | `pc-backend` | `pc-webapi` |

#### 财务服务相关主题示例

| 主题名称 | 发布者 | 订阅者 | 说明 |
|----------|--------|--------|------|
| `purchase-backend-all-orderconfirmed` | 采购管理 | 财务等 | 采购订单确认 |
| `sia-backend-all-goodsreceived` | 入库申请 | 财务等 | 货物入库完成 |
| `soa-backend-all-goodsissued` | 出库申请 | 财务等 | 货物出库完成 |
| `sell-backend-all-orderconfirmed` | 销售管理 | 财务等 | 销售订单确认 |
| `inventory-backend-all-countcompleted` | 库存管理 | 财务等 | 盘点完成 |
| `finance-backend-all-receivablesettled` | 财务管理 | 销售等 | 应收核销完成 |
| `finance-backend-all-payablesettled` | 财务管理 | 采购等 | 应付核销完成 |

## 集成边界

### 上游依赖（消费）

| 来源模块 | AppId | 事件/接口 | 用途 |
|----------|-------|-----------|------|
| 采购管理 | `purchase-backend` | `purchase-backend-all-orderconfirmed` | 生成应付单 |
| 入库申请 | `sia-backend` | `sia-backend-all-goodsreceived` | 暂估应付入账 |
| 出库申请 | `soa-backend` | `soa-backend-all-goodsissued` | 触发应收确认 |
| 销售管理 | `sell-backend` | `sell-backend-all-orderconfirmed` | 生成应收单 |
| 库存管理 | `inventory-backend` | `inventory-backend-all-countcompleted` | 盘点差异财务处理 |
| 首营中心 | `fm-webapi` | HTTP API | 供应商/客户查询 |
| 物流管理 | `logistics-backend` | `logistics-backend-all-deliveryconfirmed` | 物流费用应付 |

### 下游提供（暴露）

| 接口/事件 | 消费方 | AppId | 说明 |
|-----------|--------|-------|------|
| 应收余额查询 | 销售管理 | `sell-backend` | 客户信用额度校验 |
| 应付余额查询 | 采购管理 | `purchase-backend` | 供应商付款状态 |
| 收款状态查询 | 订货系统 | `ordering-backend` | 订单收款确认 |
| 付款状态查询 | 采购管理 | `purchase-backend` | 采购付款进度 |
| 账龄分析 | 项目管理 | `pm-backend` | 项目应收应付分析 |
| `finance-backend-all-receivablesettled` | 销售管理等 | - | 应收核销完成事件 |
| `finance-backend-all-payablesettled` | 采购管理等 | - | 应付核销完成事件 |
