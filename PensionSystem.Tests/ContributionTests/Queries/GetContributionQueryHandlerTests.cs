using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using PensionSystem.Domain.Entities;

public class GetContributionQueryHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GetContributionQueryHandler _handler;

    public GetContributionQueryHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new GetContributionQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenValidContribution_ShouldReturnContributionDto()
    {
        // Arrange
        var contributionId = Guid.NewGuid();
        var contribution = new Contribution
        {
            Id = contributionId,
            MemberId = Guid.NewGuid(),
            Amount = 1000m,
            ContributionDate = DateTime.Now,
            IsVoluntary = false,
            IsDeleted = false
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(new List<Contribution> { contribution }.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(new List<Contribution> { contribution }.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(new List<Contribution> { contribution }.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(new List<Contribution> { contribution }.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionQuery { Id = contributionId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contribution.Id, result.Id);
        Assert.Equal(contribution.MemberId, result.MemberId);
        Assert.Equal(contribution.Amount, result.Amount);
        Assert.Equal(contribution.ContributionDate, result.ContributionDate);
        Assert.Equal(contribution.IsVoluntary, result.IsVoluntary);
    }

    [Fact]
    public async Task Handle_GivenNonExistingContribution_ShouldThrowBusinessException()
    {
        // Arrange
        var contributionId = Guid.NewGuid();

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Contribution>().AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Contribution>().AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Contribution>().AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Contribution>().GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionQuery { Id = contributionId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution not found", exception.Message);
    }

    [Fact]
    public async Task Handle_GivenDeletedContribution_ShouldThrowBusinessException()
    {
        // Arrange
        var contributionId = Guid.NewGuid();
        var contribution = new Contribution
        {
            Id = contributionId,
            MemberId = Guid.NewGuid(),
            Amount = 1000m,
            ContributionDate = DateTime.Now,
            IsVoluntary = false,
            IsDeleted = true // Marked as deleted
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(new List<Contribution> { contribution }.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(new List<Contribution> { contribution }.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(new List<Contribution> { contribution }.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(new List<Contribution> { contribution }.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionQuery { Id = contributionId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution not found", exception.Message);
    }
}
