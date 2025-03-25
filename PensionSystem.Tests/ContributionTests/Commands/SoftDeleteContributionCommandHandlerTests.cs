using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using PensionSystem.Domain.Entities;
using System.Linq.Expressions;

public class SoftDeleteContributionCommandHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<DbSet<Contribution>> _mockDbSet;
    private readonly SoftDeleteContributionCommandHandler _handler;

    public SoftDeleteContributionCommandHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _mockDbSet = new Mock<DbSet<Contribution>>();
        _mockContext.Setup(m => m.Contributions).Returns(_mockDbSet.Object);
        _handler = new SoftDeleteContributionCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenValidContribution_ShouldSoftDeleteContribution()
    {
        // Arrange
        var request = new SoftDeleteContributionCommand { Id = Guid.NewGuid() };
        var contribution = new Contribution
        {
            Id = request.Id,
            IsDeleted = false
        };

        _mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Contribution, bool>>>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(contribution);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.True(contribution.IsDeleted); // The contribution should be marked as deleted
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Ensure SaveChangesAsync was called
    }

    [Fact]
    public async Task Handle_GivenNonExistingContribution_ShouldThrowBusinessException()
    {
        // Arrange
        var request = new SoftDeleteContributionCommand { Id = Guid.NewGuid() };

        _mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Contribution, bool>>>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Contribution)null); // Simulate no contribution found

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution not found", exception.Message);
    }

    [Fact]
    public async Task Handle_GivenAlreadyDeletedContribution_ShouldNotCallSaveChanges()
    {
        // Arrange
        var request = new SoftDeleteContributionCommand { Id = Guid.NewGuid() };
        var contribution = new Contribution
        {
            Id = request.Id,
            IsDeleted = true
        };

        _mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Contribution, bool>>>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(contribution);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); // No save should be called as it's already deleted
    }
}
