using MediatR;
using PensionSystem.Application.DTOs;

namespace PensionSystem.Application.Features.Queries;

public class CheckEligibilityQuery : IRequest<EligibilityDto>
{
    public Guid MemberId { get; set; }
}