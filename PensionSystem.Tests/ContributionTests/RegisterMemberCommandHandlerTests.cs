using Moq;
using PensionSystem.Application.DTOs;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Repositories;

public class RegisterMemberCommandHandlerTests
{
    private readonly Mock<IMemberRepository> _mockRepo;
    private readonly RegisterMemberCommandHandler _handler;

    public RegisterMemberCommandHandlerTests()
    {
        _mockRepo = new Mock<IMemberRepository>();
        _handler = new RegisterMemberCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_GivenValidMemberData_ShouldReturnMemberId()
    {
        // Arrange
        var request = new RegisterMemberCommand
        {
            Member = new Member
            {
                FullName = "John Doe",
                DateOfBirth = new DateTime(1990, 5, 1),
                Email = "johndoe@example.com",
                Phone = "123-456-7890"
            }
        };

        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 5, 1),
            Email = "johndoe@example.com",
            Phone = "123-456-7890"
        };

        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Member>()))
          .Returns((Member member) => member); // Returns the same member back as a result


        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.Equal(member.Id, result);
        _mockRepo.Verify(repo => repo.AddAsync(It.Is<Member>(m => m.FullName == "John Doe" && m.Email == "johndoe@example.com")), Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidMemberData_ShouldThrowException()
    {
        // Arrange
        var request = new RegisterMemberCommand
        {
            Member = new Member
            {
                FullName = "", // Invalid data (empty name)
                DateOfBirth = new DateTime(1990, 5, 1),
                Email = "invalid@example.com",
                Phone = "123-456-7890"
            }
        };

        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Member>()))
                 .ThrowsAsync(new Exception("Invalid data"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("Invalid data", exception.Message);
    }
}
