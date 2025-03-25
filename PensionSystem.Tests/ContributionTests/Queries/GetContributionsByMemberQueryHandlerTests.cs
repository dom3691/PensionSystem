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
using PensionSystem.Infrastructure.ExceptionHandler;

public class GetContributionsByMemberQueryHandlerTests
{
    private readonly GetContributionsByMemberQueryHandler _handler;
    private readonly AppDbContext _context;

    public GetContributionsByMemberQueryHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new GetContributionsByMemberQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldReturnContributionDtos()
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

        var request = new GetContributionsByMemberQuery { MemberId = memberId, IsVoluntary = true };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count); // Only one voluntary contribution
        Assert.Equal(contributions[1].Id, result[0].Id);
        Assert.Equal(contributions[1].Amount, result[0].Amount);
        Assert.Equal(contributions[1].ContributionDate, result[0].ContributionDate);
        Assert.Equal(contributions[1].IsVoluntary, result[0].IsVoluntary);
    }

    [Fact]
    public async Task Handle_GivenNoMatchingContributions_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var request = new GetContributionsByMemberQuery { MemberId = memberId, IsVoluntary = false };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("No contributions found for this member", exception.Message);
    }
}
