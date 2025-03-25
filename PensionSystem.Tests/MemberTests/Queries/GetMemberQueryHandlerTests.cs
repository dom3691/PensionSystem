using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.ExceptionHandler;

public class GetMemberQueryHandlerTests
{
    private readonly GetMemberQueryHandler _handler;
    private readonly AppDbContext _context;

    public GetMemberQueryHandlerTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _handler = new GetMemberQueryHandler(_context);
    }

    [Fact]
    public async Task Handle_GivenValidMember_ShouldReturnMemberDto()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member
        {
            Id = memberId,
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "johndoe@example.com",
            Phone = "123-456-7890",
            IsDeleted = false
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();

        var request = new GetMemberQuery { Id = memberId };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(member.FullName, result.FullName);
        Assert.Equal(member.DateOfBirth, result.DateOfBirth);
        Assert.Equal(member.Email, result.Email);
        Assert.Equal(member.Phone, result.Phone);
    }

    [Fact]
    public async Task Handle_GivenNonExistingMember_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();

        var request = new GetMemberQuery { Id = memberId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Member not found", exception.Message);
    }

    [Fact]
    public async Task Handle_GivenDeletedMember_ShouldThrowBusinessException()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = new Member
        {
            Id = memberId,
            FullName = "Jane Doe",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "janedoe@example.com",
            Phone = "987-654-3210",
            IsDeleted = true // Marked as deleted
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync();

        var request = new GetMemberQuery { Id = memberId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Member not found", exception.Message);
    }
}
