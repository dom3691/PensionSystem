using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOs;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands;

public class CreateContributionCommandHandler : IRequestHandler<CreateContributionCommand, ContributionDto>
{
    private readonly AppDbContext _context;

    public CreateContributionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContributionDto> Handle(CreateContributionCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            throw new Exception("Contribution amount must be greater than zero.");
        }

        // Validate monthly contributions (one per month)
        if (!request.IsVoluntary && _context.Contributions
                .Any(c => c.MemberId == request.MemberId && c.ContributionDate.Month == request.ContributionDate.Month && !c.IsDeleted))
        {
            throw new BusinessException("You can only make one regular contribution per month.");
        }

        var memberContributions = await _context.Contributions
            .Where(c => c.MemberId == request.MemberId && !c.IsDeleted)
            .OrderBy(c => c.ContributionDate)
            .ToListAsync();

        // Calculate the duration of contributions for this member
        if (memberContributions.Count >= 1)
        {
        }
        else
        {
            // Not eligible yet
            throw new BusinessException("You must have contributed for at least 1 months to be eligible for benefits.");
        }

        var contribution = new Contribution
        {
            MemberId = request.MemberId,
            Amount = request.Amount,
            ContributionDate = request.ContributionDate,
            IsVoluntary = request.IsVoluntary
        };

        _context.Contributions.Add(contribution);
        await _context.SaveChangesAsync(cancellationToken);

        return new ContributionDto
        {
           // Id = contribution.Id,
            MemberId = contribution.MemberId,
            Amount = contribution.Amount,
            ContributionDate = contribution.ContributionDate,
            IsVoluntary = contribution.IsVoluntary
        };
    }
}
