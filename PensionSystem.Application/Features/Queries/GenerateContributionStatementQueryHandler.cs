using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries
{
    public class GenerateContributionStatementQueryHandler : IRequestHandler<GenerateContributionStatementQuery, string>
    {
        private readonly AppDbContext _context;

        public GenerateContributionStatementQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GenerateContributionStatementQuery request, CancellationToken cancellationToken)
        {
            var contributions = await _context.Contributions
                .Where(c => c.MemberId == request.MemberId && !c.IsDeleted)
                .OrderBy(c => c.ContributionDate)
                .ToListAsync(cancellationToken);

            if (!contributions.Any())
            {
                throw new BusinessException("No contributions found for this member");
            }

            var statement = new StringBuilder();
            statement.AppendLine($"Contribution Statement for Member ID: {request.MemberId}");
            statement.AppendLine("---------------------------------------------------");
            statement.AppendLine("Date    |      Amount     | Type");

            foreach (var contribution in contributions)
            {
                statement.AppendLine($"{contribution.ContributionDate.ToString("yyyy-MM-dd")} | {contribution.Amount:C} | {(contribution.IsVoluntary ? "Voluntary" : "Monthly")}");
            }

            statement.AppendLine("---------------------------------------------------");
            statement.AppendLine($"Total Contributions: {contributions.Sum(c => c.Amount):C}");

            return statement.ToString(); 
        }
    }
}
