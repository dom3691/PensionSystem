using Microsoft.EntityFrameworkCore;
using Moq;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Tests.ContributionTests.Commands
{
    public class CreateContributionCommandHandlerTests
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly CreateContributionCommandHandler _handler;
        public CreateContributionCommandHandlerTests()
        {
            _mockContext = new Mock<AppDbContext>();
            _handler = new CreateContributionCommandHandler(_mockContext.Object);
        }


        [Fact]
        public async Task Handle_GivenValidContribution_ShouldAddContribution()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var contributionDate = new DateTime(2025, 3, 15);
            var amount = 1000m;

            var request = new CreateContributionCommand
            {
                MemberId = memberId,
                ContributionDate = contributionDate,
                Amount = amount,
                IsVoluntary = false
            };

            var contributions = new List<Contribution>
        {
            new Contribution
            {
                MemberId = memberId,
                ContributionDate = new DateTime(2025, 2, 15),
                Amount = 500m,
                IsVoluntary = false,
                IsDeleted = false
            }
        };

            var dbSetMock = new Mock<DbSet<Contribution>>();
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

            _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(memberId, result.MemberId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(contributionDate, result.ContributionDate);
            Assert.False(result.IsVoluntary);
        }

        [Fact]
        public async Task Handle_GivenZeroAmount_ShouldThrowException()
        {
            // Arrange
            var request = new CreateContributionCommand
            {
                MemberId = Guid.NewGuid(),
                ContributionDate = DateTime.Now,
                Amount = 0, // Invalid amount
                IsVoluntary = false
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("Contribution amount must be greater than zero.", exception.Message);
        }

        [Fact]
        public async Task Handle_GivenDuplicateContributionForTheMonth_ShouldThrowBusinessException()
        {
            // Arrange
            var request = new CreateContributionCommand
            {
                MemberId = Guid.NewGuid(),
                ContributionDate = new DateTime(2025, 3, 15),
                Amount = 1000m,
                IsVoluntary = false
            };

            var contributions = new List<Contribution>
        {
            new Contribution
            {
                MemberId = Guid.NewGuid(),
                ContributionDate = new DateTime(2025, 3, 1),
                Amount = 500m,
                IsVoluntary = false,
                IsDeleted = false
            }
        };

            var dbSetMock = new Mock<DbSet<Contribution>>();
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

            _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("You can only make one regular contribution per month.", exception.Message);
        }

        [Fact]
        public async Task Handle_GivenNoPreviousContributions_ShouldThrowBusinessException()
        {
            // Arrange
            var request = new CreateContributionCommand
            {
                MemberId = Guid.NewGuid(),
                ContributionDate = DateTime.Now,
                Amount = 1000m,
                IsVoluntary = false
            };

            var contributions = new List<Contribution>();

            var dbSetMock = new Mock<DbSet<Contribution>>();
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Provider).Returns(contributions.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.Expression).Returns(contributions.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.ElementType).Returns(contributions.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Contribution>>().Setup(m => m.GetEnumerator()).Returns(contributions.GetEnumerator());

            _mockContext.Setup(c => c.Contributions).Returns(dbSetMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("You must have contributed for at least 1 months to be eligible for benefits.", exception.Message);
        }

    }
}
