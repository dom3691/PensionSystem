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

public class GetContributionQueryHandlerTests
{
    private readonly GetContributionQueryHandler _handler;
    private readonly AppDbContext _context;

    public GetContributionQueryHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new GetContributionQueryHandler(_context);
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

        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();

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

        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();

        var request = new GetContributionQuery { Id = contributionId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution not found", exception.Message);
    }
}
