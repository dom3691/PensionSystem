using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.ExceptionHandler;

public class SoftDeleteContributionCommandHandlerTests
{
    private readonly SoftDeleteContributionCommandHandler _handler;
    private readonly AppDbContext _context;

    public SoftDeleteContributionCommandHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new SoftDeleteContributionCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenAlreadyDeletedContribution_ShouldNotCallSaveChanges()
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
            IsDeleted = true // Already deleted
        };

        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();

        var request = new SoftDeleteContributionCommand { Id = contributionId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedContribution = await _context.Contributions.FindAsync(contributionId);
        Assert.True(updatedContribution.IsDeleted); // Ensure it's still deleted
    }

    [Fact]
    public async Task Handle_GivenValidContribution_ShouldSoftDeleteContribution()
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
            IsDeleted = false // Not deleted
        };

        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync();

        var request = new SoftDeleteContributionCommand { Id = contributionId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedContribution = await _context.Contributions.FindAsync(contributionId);
        Assert.True(updatedContribution.IsDeleted); // Ensure it's marked as deleted
    }

    [Fact]
    public async Task Handle_GivenNonExistingContribution_ShouldThrowBusinessException()
    {
        // Arrange
        var contributionId = Guid.NewGuid();
        var request = new SoftDeleteContributionCommand { Id = contributionId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution not found", exception.Message);
    }
}
