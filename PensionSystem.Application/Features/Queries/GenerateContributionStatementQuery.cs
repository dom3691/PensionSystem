using MediatR;

namespace PensionSystem.Application.Features.Queries;

public class GenerateContributionStatementQuery : IRequest<string>
{
    public Guid MemberId { get; set; }
}
