using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;

public class GetContributionSummaryRequestHandlerTests
{
    private readonly GetContributionSummaryRequestHandler _handler;
    private readonly AppDbContext _context;

    public GetContributionSummaryRequestHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new GetContributionSummaryRequestHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldReturnContributionSummaryDto()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { Id = Guid.NewGuid(), MemberId = memberId, Amount = 1000m, ContributionDate = DateTime.Now.AddMonths(-1), IsVoluntary = false, IsDeleted = false },
            new Contribution { Id = Guid.NewGuid(), MemberId = memberId, Amount = 500m, ContributionDate = DateTime.Now.AddMonths(-2), IsVoluntary = true, IsDeleted = false }
        };

        _context.Contributions.AddRange(contributions);
        await _context.SaveChangesAsync();

        var request = new GetContributionSummaryQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1500m, result.TotalContributions);
        Assert.Equal(2, result.Contributions.Count);
        Assert.Equal(contributions[0].Id, result.Contributions[0].Id);
        Assert.Equal(contributions[1].Id, result.Contributions[1].Id);
    }

    [Fact]
    public async Task Handle_GivenNoContributions_ShouldReturnEmptySummary()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var request = new GetContributionSummaryQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.TotalContributions);
        Assert.Empty(result.Contributions);
    }
}
