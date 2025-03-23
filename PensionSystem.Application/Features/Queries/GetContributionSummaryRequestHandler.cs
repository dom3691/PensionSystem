using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries
{
    public class GetContributionSummaryRequestHandler : IRequestHandler<GetContributionSummaryQuery, ContributionSummaryDto>
    {
        private readonly AppDbContext _context;

        public GetContributionSummaryRequestHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ContributionSummaryDto> Handle(GetContributionSummaryQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _context.Contributions
            .Where(c => c.MemberId == request.MemberId && !c.IsDeleted)
            .ToListAsync();

        var totalContributions = contributions.Sum(c => c.Amount);
        return new ContributionSummaryDto
        {
            TotalContributions = totalContributions,
            Contributions = contributions.Select(c => new ContributionDto
            {
                Id = c.Id,
                Amount = c.Amount,
                ContributionDate = c.ContributionDate,
                MemberId = c.MemberId,
                IsVoluntary = c.IsVoluntary
            }).ToList()
        };
    }
    }
}
