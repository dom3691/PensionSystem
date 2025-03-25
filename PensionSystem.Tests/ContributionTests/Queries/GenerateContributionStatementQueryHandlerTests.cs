using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using PensionSystem.Domain.Entities;

public class GenerateContributionStatementQueryHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GenerateContributionStatementQueryHandler _handler;

    public GenerateContributionStatementQueryHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new GenerateContributionStatementQueryHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenContributions_ShouldGenerateStatement()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contributions = new List<Contribution>
        {
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-1), Amount = 1000m, IsVoluntary = false, IsDeleted = false },
            new Contribution { MemberId = memberId, ContributionDate = DateTime.Now.AddMonths(-2), Amount = 500m, IsVoluntary = true, IsDeleted = false }
        };

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

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

    [Fact]
    public async Task Handle_GivenNoContributions_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var dbSetMock = new Mock<DbSet<Contribution>>();
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Contribution>().AsQueryable().Provider);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Contribution>().AsQueryable().Expression);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Contribution>().AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Contribution>().GetEnumerator());

        _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

        var request = new GenerateContributionStatementQuery { MemberId = memberId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("No contributions found for this member", exception.Message);
    }
}
