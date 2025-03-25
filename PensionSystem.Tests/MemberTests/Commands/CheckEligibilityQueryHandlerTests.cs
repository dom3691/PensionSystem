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

public class CheckEligibilityQueryHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly CheckEligibilityQueryHandler _handler;

    public CheckEligibilityQueryHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new CheckEligibilityQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenMemberWithSixOrMoreContributions_ShouldReturnEligible()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-1), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-2), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-3), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-4), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-5), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-6), IsDeleted = false }
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new CheckEligibilityQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsEligible);
        Assert.Equal("Eligible for benefits", result.Message);
    }

    [Fact]
    public async Task Handle_GivenMemberWithLessThanSixContributions_ShouldReturnNotEligible()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-1), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-2), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-3), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-4), IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-5), IsDeleted = false }
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new CheckEligibilityQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsEligible);
        Assert.Equal("Not eligible for benefits yet. You need to contribute for at least 6 months.", result.Message);
    }
}
