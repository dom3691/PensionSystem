using Microsoft.EntityFrameworkCore;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.BackgroundJobs
{
    public class InterestCalculationJob
    {
        private readonly AppDbContext _context;

        public InterestCalculationJob(AppDbContext context)
        {
            _context = context;
        }

        public async Task CalculateInterestAsync()
        {
            var contributions = await _context.Contributions
                .Where(c => !c.IsDeleted && c.IsValidated)
                .ToListAsync();

            foreach (var contribution in contributions)
            {
                // Example interest calculation logic (simple example)
                decimal interestRate = 0.05m;  // 5% interest rate
                decimal interest = contribution.Amount * interestRate;
                contribution.Amount += interest;

                _context.Contributions.Update(contribution);
            }

            await _context.SaveChangesAsync();
        }
    }
}
