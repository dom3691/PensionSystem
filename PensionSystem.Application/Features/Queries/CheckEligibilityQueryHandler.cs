using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries;

public class CheckEligibilityQueryHandler : IRequestHandler<CheckEligibilityQuery, EligibilityDto>
{
    private readonly AppDbContext _context;

    public CheckEligibilityQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EligibilityDto> Handle(CheckEligibilityQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _context.Contributions
            .Where(c => c.MemberId == request.MemberId && !c.IsDeleted)
            .OrderBy(c => c.ContributionDate)
            .ToListAsync();

        // Business rule: 6 months minimum contribution
        if (contributions.Count >= 6)
        {
            return new EligibilityDto
            {
                IsEligible = true,
                Message = "Eligible for benefits"
            };
        }

        return new EligibilityDto
        {
            IsEligible = false,
            Message = "Not eligible for benefits yet. You need to contribute for at least 6 months."
        };
    }
}
