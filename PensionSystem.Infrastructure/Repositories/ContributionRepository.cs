using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Infrastructure.Repositories;

public class ContributionRepository : IContributionRepository
{
    private readonly AppDbContext _context;
    public ContributionRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Contribution contribution)
    {
        await _context.Contributions.AddAsync(contribution);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Contribution>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.Contributions.Where(x => x.MemberId == memberId).ToListAsync();
    }
}
