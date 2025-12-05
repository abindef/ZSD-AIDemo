using FluentAssertions;
using Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;
using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Inno.CorePlatform.Finance.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Inno.CorePlatform.Finance.Application.Tests.Commands;

public class ClaimReceivableHandlerTests
{
    private readonly Mock<IReceivableRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ClaimReceivableHandler _handler;

    public ClaimReceivableHandlerTests()
    {
        _repositoryMock = new Mock<IReceivableRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new ClaimReceivableHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSucceed()
    {
        // Arrange
        var receivable = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(1000m, 13),
            "CUST001",
            PaymentTerm.Net30);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ReceivableId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receivable);

        var command = new ClaimReceivableCommand(
            receivable.Id.Value,
            500m,
            13,
            "BankTransfer");

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReceivableNotFound_ShouldFail()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ReceivableId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Receivable?)null);

        var command = new ClaimReceivableCommand(
            Guid.NewGuid(),
            500m,
            13,
            "BankTransfer");

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be("RECEIVABLE_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_WithInvalidClaimType_ShouldFail()
    {
        // Arrange
        var receivable = Receivable.Create(
            ReceivableId.Create(),
            Money.Create(1000m, 13),
            "CUST001",
            PaymentTerm.Net30);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ReceivableId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(receivable);

        var command = new ClaimReceivableCommand(
            receivable.Id.Value,
            500m,
            13,
            "InvalidType");

        // Act
        var result = await _handler.Handle(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Code.Should().Be("INVALID_CLAIM_TYPE");
    }
}
