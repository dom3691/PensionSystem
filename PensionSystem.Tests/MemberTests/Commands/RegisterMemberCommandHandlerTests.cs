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
