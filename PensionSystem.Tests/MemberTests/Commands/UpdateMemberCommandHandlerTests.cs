using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;

public class UpdateMemberCommandHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly UpdateMemberCommandHandler _handler;

    public UpdateMemberCommandHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new UpdateMemberCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_GivenValidMemberId_ShouldUpdateMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var request = new UpdateMemberCommand
        {
            Id = memberId,
            FullName = "Updated FullName",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "updatedemail@example.com",
            Phone = "098-765-4321"
        };

        var member = new Member
        {
            Id = memberId,
            FullName = "Original FullName",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "originalemail@example.com",
            Phone = "123-456-7890"
        };

        // Mock the DbSet and the context
        var dbSetMock = new Mock<DbSet<Member>>();
        dbSetMock.Setup(m => m.FindAsync(It.IsAny<Guid>())).ReturnsAsync(member);

        _mockContext.Setup(c => c.Members).Returns(dbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.DateOfBirth, result.DateOfBirth);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);

        _mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenNonExistingMember_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateMemberCommand
        {
            Id = Guid.NewGuid(),
            FullName = "Updated FullName",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "updatedemail@example.com",
            Phone = "098-765-4321"
        };

        _mockContext.Setup(c => c.Members.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Member)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Member not found", exception.Message);
    }
}
