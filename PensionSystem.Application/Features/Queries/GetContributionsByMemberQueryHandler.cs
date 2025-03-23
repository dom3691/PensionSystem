using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries;

public class GetContributionsByMemberQueryHandler : IRequestHandler<GetContributionsByMemberQuery, List<ContributionDto>>
{
    private readonly AppDbContext _context;

    public GetContributionsByMemberQueryHandler(AppDbContext context)
    {
        _context = context;

    }

    public async Task<List<ContributionDto>> Handle(GetContributionsByMemberQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _context.Contributions
            .Where(c => c.MemberId == request.MemberId && c.IsVoluntary == request.IsVoluntary && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!contributions.Any())
            throw new BusinessException("No contributions found for this member");

        return contributions.Select(c => new ContributionDto
        {
            Id = c.Id,
            MemberId = c.MemberId,
            Amount = c.Amount,
            ContributionDate = c.ContributionDate,
            IsVoluntary = c.IsVoluntary
        }).ToList();
    }

}
