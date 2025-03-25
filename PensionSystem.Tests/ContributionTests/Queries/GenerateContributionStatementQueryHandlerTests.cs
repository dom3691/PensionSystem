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
using System.Text;

public class GenerateContributionStatementQueryHandlerTests
{
    private readonly GenerateContributionStatementQueryHandler _handler;
    private readonly AppDbContext _context;

    public GenerateContributionStatementQueryHandlerTests()
    {
        // Use In-Memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new GenerateContributionStatementQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldGenerateStatement()
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

        var request = new GenerateContributionStatementQuery { MemberId = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var expectedStatement = new StringBuilder()
            .AppendLine($"Contribution Statement for Member ID: {memberId}")
            .AppendLine("---------------------------------------------------")
            .AppendLine("Date    |      Amount     | Type")
            .AppendLine($"{contributions[0].ContributionDate:yyyy-MM-dd} | {contributions[0].Amount:C} | Monthly")
            .AppendLine($"{contributions[1].ContributionDate:yyyy-MM-dd} | {contributions[1].Amount:C} | Voluntary")
            .AppendLine("---------------------------------------------------")
            .AppendLine($"Total Contributions: {contributions.Sum(c => c.Amount):C}")
            .ToString();

        Assert.Equal(expectedStatement, result);
    }

}
