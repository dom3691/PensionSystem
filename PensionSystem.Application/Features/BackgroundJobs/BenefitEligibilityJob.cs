using Microsoft.EntityFrameworkCore;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Application.Features.BackgroundJobs
{
    public class BenefitEligibilityJob
    {
        private readonly AppDbContext _context;

        public BenefitEligibilityJob(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateBenefitEligibilityAsync()
        {
            var members = await _context.Members.ToListAsync();

            foreach (var member in members)
            {
                var contributions = await _context.Contributions
                    .Where(c => c.MemberId == member.Id && !c.IsDeleted)
                    .ToListAsync();

                if (contributions.Count >= 1)
                {
                    // Mark member as eligible for benefits
                    member.IsEligibleForBenefits = true;
                }
                else
                {
                    member.IsEligibleForBenefits = false;
                }

                _context.Members.Update(member);
            }

            await _context.SaveChangesAsync();
        }
    }
}
