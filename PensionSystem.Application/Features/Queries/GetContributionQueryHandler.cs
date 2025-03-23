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

public class GetContributionQueryHandler : IRequestHandler<GetContributionQuery, ContributionDto>
{
    private readonly AppDbContext _context;

    public GetContributionQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContributionDto> Handle(GetContributionQuery request, CancellationToken cancellationToken)
    {
        var contribution = await _context.Contributions
            .Where(c => c.Id == request.Id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (contribution == null)
            throw new BusinessException("Contribution not found");

        return new ContributionDto
        {
            Id = contribution.Id,
            MemberId = contribution.MemberId,
            Amount = contribution.Amount,
            ContributionDate = contribution.ContributionDate,
            IsVoluntary = contribution.IsVoluntary
        };
    }
}
