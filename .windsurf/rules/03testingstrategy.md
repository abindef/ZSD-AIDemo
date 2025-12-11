---
trigger: manual
---

# 测试策略 (Testing Strategy)

<testing_stack>
  - Framework: xUnit
  - Mocking: Moq
  - Assertions: FluentAssertions
</testing_stack>

<testing_requirements>
  <layer_coverage>
    1. **Domain Tests**: **必须 100% 覆盖**。重点测试聚合根状态流转、Money 计算精度、冲销逻辑。不依赖数据库。
    2. **Application Tests**: 重点测试 UseCase 流程、Dapr 事件发布验证。使用 Mock 模拟 Repository。
  </layer_coverage>

  <naming_convention>
    格式: `MethodName_Scenario_ExpectedResult`
    示例: `Claim_WithSettledReceivable_ShouldThrowBusinessException`
  </naming_convention>
</testing_requirements>

<checklist>
  提交代码前必须验证：
  - [ ] 是否处理了金额的 2位/10位 精度？
  - [ ] 冲销逻辑是否校验了 4 个维度（项目/BU/供应商/公司）？
  - [ ] 认款是否检查了互斥规则？
  - [ ] 单元测试是否断言了领域事件的产生？
</checklist>