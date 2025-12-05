# 测试用例设计模板

> Tester 设计测试用例的标准化模板

---

## 元信息

| 字段 | 值 |
|------|-----|
| **测试目标** | [被测试的类/方法/功能] |
| **关联领域模型** | [链接] |
| **关联代码请求** | [链接] |
| **设计人** | [Tester] |
| **日期** | [日期] |

---

## 一、测试范围

### 1.1 被测对象

| 类型 | 名称 | 路径 |
|------|------|------|
| 聚合根 | [类名] | [文件路径] |
| 应用服务 | [类名] | [文件路径] |
| API | [路径] | [Controller] |

### 1.2 测试类型

- [ ] 单元测试（Domain层）
- [ ] 集成测试（Application层）
- [ ] API测试（Host层）
- [ ] 端到端测试

---

## 二、业务规则测试矩阵

| 规则编号 | 规则描述 | 测试用例编号 | 覆盖状态 |
|----------|----------|--------------|----------|
| BR-001 | [规则描述] | TC-001, TC-002 | ☐ 已覆盖 |

---

## 三、测试用例设计

### 3.1 正常场景

#### TC-001: [测试用例名称]

| 字段 | 值 |
|------|-----|
| **场景描述** | [必填] |
| **前置条件** | [必填] |
| **测试数据** | [见下方] |
| **执行步骤** | [见下方] |
| **期望结果** | [必填] |
| **优先级** | [P0/P1/P2] |

**测试数据**：
```csharp
var input = new TestInput
{
    Field1 = "value1",
    Field2 = 100
};
```

**执行步骤**：
1. 准备测试数据
2. 调用被测方法
3. 验证返回结果

**测试代码**：
```csharp
[Fact]
public void [MethodName]_[Scenario]_[ExpectedResult]()
{
    // Arrange
    var sut = CreateSystemUnderTest();
    var input = new TestInput { /* ... */ };
    
    // Act
    var result = sut.Method(input);
    
    // Assert
    result.Should().NotBeNull();
    result.Property.Should().Be(expectedValue);
}
```

---

### 3.2 边界场景

#### TC-B001: [边界测试用例名称]

| 字段 | 值 |
|------|-----|
| **边界类型** | [空值/最大值/最小值/临界值] |
| **场景描述** | [必填] |
| **测试数据** | [边界值] |
| **期望结果** | [必填] |

**测试代码**：
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void [MethodName]_WithInvalidInput_ShouldThrow(string input)
{
    // Arrange
    var sut = CreateSystemUnderTest();
    
    // Act & Assert
    var act = () => sut.Method(input);
    act.Should().Throw<ArgumentException>();
}
```

---

### 3.3 异常场景

#### TC-E001: [异常测试用例名称]

| 字段 | 值 |
|------|-----|
| **异常类型** | [异常类名] |
| **触发条件** | [必填] |
| **期望行为** | [抛出异常/返回错误码] |
| **错误消息** | [期望的错误消息] |

**测试代码**：
```csharp
[Fact]
public void [MethodName]_[ExceptionCondition]_ShouldThrow[ExceptionType]()
{
    // Arrange
    var sut = CreateSystemUnderTest();
    var invalidInput = CreateInvalidInput();
    
    // Act
    var act = () => sut.Method(invalidInput);
    
    // Assert
    act.Should().Throw<BusinessException>()
       .WithMessage("*期望的错误消息*");
}
```

---

### 3.4 状态流转测试

#### TC-S001: [状态流转测试用例名称]

| 字段 | 值 |
|------|-----|
| **初始状态** | [状态名] |
| **触发操作** | [方法名] |
| **期望状态** | [状态名] |
| **触发事件** | [事件名] |

**测试代码**：
```csharp
[Fact]
public void [AggregateRoot]_[Operation]_ShouldTransitionTo[NewState]()
{
    // Arrange
    var aggregate = CreateAggregateInState(InitialState);
    
    // Act
    aggregate.PerformOperation();
    
    // Assert
    aggregate.Status.Should().Be(ExpectedState);
    aggregate.DomainEvents.Should().ContainSingle()
        .Which.Should().BeOfType<ExpectedEvent>();
}
```

---

### 3.5 并发测试（如适用）

#### TC-C001: [并发测试用例名称]

| 字段 | 值 |
|------|-----|
| **并发场景** | [描述] |
| **并发数** | [数量] |
| **期望行为** | [只有一个成功/全部成功/幂等] |

---

## 四、Mock策略

### 4.1 需要Mock的依赖

| 依赖接口 | Mock策略 | 返回值设置 |
|----------|----------|------------|
| IRepository | Mock | 返回预设实体 |
| IExternalService | Mock | 返回成功响应 |

### 4.2 Mock代码示例

```csharp
private Mock<IReceivableRepository> _mockRepository;

private void SetupMocks()
{
    _mockRepository = new Mock<IReceivableRepository>();
    _mockRepository
        .Setup(x => x.GetByIdAsync(It.IsAny<ReceivableId>()))
        .ReturnsAsync(CreateTestReceivable());
}
```

---

## 五、测试数据工厂

```csharp
public static class TestDataFactory
{
    public static Receivable CreateReceivable(
        decimal amount = 100m,
        ReceivableStatus status = ReceivableStatus.Pending)
    {
        return Receivable.Create(
            ReceivableId.New(),
            Money.Create(amount, taxRate: 13),
            // ... 其他参数
        );
    }
}
```

---

## 六、测试覆盖率要求

| 覆盖类型 | 目标 | 当前 |
|----------|------|------|
| 行覆盖率 | ≥80% | [%] |
| 分支覆盖率 | ≥70% | [%] |
| 方法覆盖率 | 100% | [%] |

---

## 七、测试执行

### 7.1 执行命令

```bash
# 运行所有测试
dotnet test

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~ReceivableTests"

# 生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
```

### 7.2 CI/CD集成

```yaml
- name: Run Tests
  run: dotnet test --configuration Release --no-build --verbosity normal
```

---

## 八、测试用例检查清单

- [ ] 所有业务规则都有对应测试用例
- [ ] 正常/边界/异常场景都已覆盖
- [ ] 状态流转测试完整
- [ ] Mock策略合理
- [ ] 测试数据工厂已创建
- [ ] 测试命名符合规范
