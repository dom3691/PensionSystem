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
using PensionSystem.Domain.Entities;

public class GetContributionSummaryRequestHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GetContributionSummaryRequestHandler _handler;

    public GetContributionSummaryRequestHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new GetContributionSummaryRequestHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldReturnContributionSummaryDto()
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

        var request = new GetContributionSummaryQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1500m, result.TotalContributions);
        Assert.Equal(2, result.Contributions.Count);

        var contribution1 = result.Contributions[0];
        Assert.Equal(contributions[0].Id, contribution1.Id);
        Assert.Equal(contributions[0].Amount, contribution1.Amount);
        Assert.Equal(contributions[0].ContributionDate, contribution1.ContributionDate);
        Assert.Equal(contributions[0].IsVoluntary, contribution1.IsVoluntary);

        var contribution2 = result.Contributions[1];
        Assert.Equal(contributions[1].Id, contribution2.Id);
        Assert.Equal(contributions[1].Amount, contribution2.Amount);
        Assert.Equal(contributions[1].ContributionDate, contribution2.ContributionDate);
        Assert.Equal(contributions[1].IsVoluntary, contribution2.IsVoluntary);
    }

    [Fact]
    public async Task Handle_GivenNoContributions_ShouldReturnEmptySummary()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Contribution>().AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Contribution>().AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Contribution>().AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Contribution>().GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GetContributionSummaryQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.TotalContributions);
        Assert.Empty(result.Contributions);
    }
}
