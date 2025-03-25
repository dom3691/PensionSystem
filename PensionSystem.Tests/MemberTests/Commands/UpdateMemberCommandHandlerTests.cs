using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.ExceptionHandler;

public class UpdateMemberCommandHandlerTests
{
    private readonly UpdateMemberCommandHandler _handler;
    private readonly AppDbContext _context;

    public UpdateMemberCommandHandlerTests()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new UpdateMemberCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenValidMemberId_ShouldUpdateMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member
        {
            Id = memberId,
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "johndoe@example.com",
            Phone = "123-456-7890"
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();

        var request = new UpdateMemberCommand
        {
            Id = memberId,
            FullName = "John Updated",
            DateOfBirth = new DateTime(1991, 5, 1),
            Email = "johnupdated@example.com",
            Phone = "987-654-3210"
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var updatedMember = await _context.Members.FindAsync(memberId);
        Assert.Equal("John Updated", updatedMember.FullName);
        Assert.Equal(new DateTime(1991, 5, 1), updatedMember.DateOfBirth);
        Assert.Equal("johnupdated@example.com", updatedMember.Email);
        Assert.Equal("987-654-3210", updatedMember.Phone);
    }

    [Fact]
    public async Task Handle_GivenNonExistingMember_ShouldThrowException()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var request = new UpdateMemberCommand
        {
            Id = memberId,
            FullName = "Non Existing Member",
            DateOfBirth = new DateTime(1991, 5, 1),
            Email = "nonexisting@example.com",
            Phone = "000-000-0000"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Member not found", exception.Message);
    }
}
