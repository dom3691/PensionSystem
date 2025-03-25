using Microsoft.EntityFrameworkCore;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.BackgroundJobs
{
    public class BackgroundJobs
    {
        public class ContributionValidationJob
        {
            private readonly AppDbContext _context;

            public ContributionValidationJob(AppDbContext context)
            {
                _context = context;
            }

            public async Task ValidateContributionsAsync()
            {
                var contributions = await _context.Contributions
                    .Where(c => !c.IsDeleted && !c.IsValidated)
                    .ToListAsync();

                foreach (var contribution in contributions)
                {
                    // Example of validation: Ensure monthly contributions are only once per month
                    var existingContribution = await _context.Contributions
                        .Where(c => c.MemberId == contribution.MemberId
                                    && c.ContributionDate.Month == contribution.ContributionDate.Month
                                    && !c.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (existingContribution != null)
                    {
                        // Logic for marking the contribution as invalid
                        contribution.IsValidated = false;
                        _context.Contributions.Update(contribution);
                    }
                    else
                    {
                        contribution.IsValidated = true;
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

    }
}
