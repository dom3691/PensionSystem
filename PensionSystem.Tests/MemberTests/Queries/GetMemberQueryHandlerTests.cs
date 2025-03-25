using Microsoft.EntityFrameworkCore;
using Moq;
using PensionSystem.Application.Features.Queries;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;

public class GetMemberQueryHandlerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GetMemberQueryHandler _handler;

    public GetMemberQueryHandlerTests()
    {
        _mockContext = new Mock<AppDbContext>();
        _handler = new GetMemberQueryHandler(_mockContext.Object);
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

        var dbSetMock = new Mock<DbSet<Member>>();
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(new List<Member> { member }.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(new List<Member> { member }.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(new List<Member> { member }.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(new List<Member> { member }.GetEnumerator());

        _mockContext.Setup(c => c.Members).Returns(dbSetMock.Object);

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

        var dbSetMock = new Mock<DbSet<Member>>();
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Member>().AsQueryable().Provider);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Member>().AsQueryable().Expression);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Member>().AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Member>().GetEnumerator());

        _mockContext.Setup(c => c.Members).Returns(dbSetMock.Object);

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
            IsDeleted = true // Marked as 
        };

        var dbSetMock = new Mock<DbSet<Member>>();
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Provider).Returns(new List<Member> { member }.AsQueryable().Provider);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.Expression).Returns(new List<Member> { member }.AsQueryable().Expression);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.ElementType).Returns(new List<Member> { member }.AsQueryable().ElementType);
        dbSetMock.As<IQueryable<Member>>().Setup(m => m.GetEnumerator()).Returns(new List<Member> { member }.GetEnumerator());

        _mockContext.Setup(c => c.Members).Returns(dbSetMock.Object);

        var request = new GetMemberQuery { Id = memberId };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Member not found", exception.Message);
    }
}
