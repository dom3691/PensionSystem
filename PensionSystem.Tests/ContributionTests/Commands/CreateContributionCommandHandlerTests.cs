using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.ExceptionHandler;

public class CreateContributionCommandHandlerTests
{
    private readonly CreateContributionCommandHandler _handler;
    private readonly AppDbContext _context;

    public CreateContributionCommandHandlerTests()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new CreateContributionCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenZeroAmount_ShouldThrowException()
    {
        // Arrange
        var request = new CreateContributionCommand
        {
            MemberId = Guid.NewGuid(),
            Amount = 0, // Invalid amount
            ContributionDate = DateTime.Now
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Contribution amount must be greater than zero.", exception.Message);
    }


    [Fact]
    public async Task Handle_GivenValidContribution_ShouldAddContribution()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        // Add a previous contribution to make the member eligible
        var previousContribution = new Contribution
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Amount = 1000m,
            ContributionDate = DateTime.Now.AddMonths(-1),
            IsVoluntary = false,
            IsDeleted = false
        };

        _context.Contributions.Add(previousContribution);
        await _context.SaveChangesAsync();

        var request = new CreateContributionCommand
        {
            MemberId = memberId,
            Amount = 500,
            ContributionDate = DateTime.Now
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var contribution = await _context.Contributions.FindAsync(result);
        Assert.NotNull(contribution);
        Assert.Equal(request.Amount, contribution.Amount);
        Assert.Equal(request.MemberId, contribution.MemberId);
    }


    [Fact]
    public async Task Handle_GivenDuplicateContributionForTheMonth_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var contribution1 = new Contribution
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Amount = 1000,
            ContributionDate = DateTime.Now.AddMonths(-1),
            IsVoluntary = false
        };

        _context.Contributions.Add(contribution1);
        await _context.SaveChangesAsync();

        var request = new CreateContributionCommand
        {
            MemberId = memberId,
            Amount = 500,
            ContributionDate = DateTime.Now.AddMonths(-1) // Duplicate month
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("You can only make one regular contribution per month.", exception.Message);
    }

    [Fact]
    public async Task Handle_GivenNoPreviousContributions_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var request = new CreateContributionCommand
        {
            MemberId = memberId,
            Amount = 500,
            ContributionDate = DateTime.Now.AddMonths(-1)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("You must have contributed for at least 1 months to be eligible for benefits.", exception.Message);
    }
}
