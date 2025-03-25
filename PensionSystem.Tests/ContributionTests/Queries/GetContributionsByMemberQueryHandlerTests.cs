using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using PensionSystem.Domain.Entities;

public class GetContributionsByMemberQueryHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GetContributionsByMemberQueryHandler _handler;

    public GetContributionsByMemberQueryHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new GetContributionsByMemberQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldReturnContributionDtos()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { Id = Guid.NewGuid(), MemberId = memberId, Amount = 1000m, ContributionDate = DateTime.Now.AddMonths(-1), IsVoluntary = true, IsDeleted = false },
            new Contribution { Id = Guid.NewGuid(), MemberId = memberId, Amount = 500m, ContributionDate = DateTime.Now.AddMonths(-2), IsVoluntary = false, IsDeleted = false }
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionsByMemberQuery { MemberId = memberId, IsVoluntary = true };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Equal(contributions[0].Id, result[0].Id);
        Assert.Equal(contributions[0].Amount, result[0].Amount);
        Assert.Equal(contributions[0].ContributionDate, result[0].ContributionDate);
        Assert.Equal(contributions[0].IsVoluntary, result[0].IsVoluntary);
    }

    [Fact]
    public async Task Handle_GivenNoMatchingContributions_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Contribution>().AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Contribution>().AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Contribution>().AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Contribution>().GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionsByMemberQuery { MemberId = memberId, IsVoluntary = false };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("No contributions found for this member", exception.Message);
    }
}
