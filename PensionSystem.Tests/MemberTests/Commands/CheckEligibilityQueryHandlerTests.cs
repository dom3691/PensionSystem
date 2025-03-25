using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using PensionSystem.Domain.Entities;

public class CheckEligibilityQueryHandlerTests
{
    private readonly CheckEligibilityQueryHandler _handler;
    private readonly AppDbContext _context;

    public CheckEligibilityQueryHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new CheckEligibilityQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenMemberWithSixOrMoreContributions_ShouldReturnEligible()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-1), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-2), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-3), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-4), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-5), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-6), IsDeleted = false, Amount = 100 }
        };

        _context.Contributions.AddRange(contributions);
        await _context.SaveChangesAsync();

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
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-1), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-2), IsDeleted = false, Amount = 100 },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-3), IsDeleted = false, Amount = 100 }
        };

        _context.Contributions.AddRange(contributions);
        await _context.SaveChangesAsync();

        var request = new CheckEligibilityQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsEligible);
        Assert.Equal("Not eligible for benefits yet. You need to contribute for at least 6 months.", result.Message);
    }
}
