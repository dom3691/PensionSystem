using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Infrastructure.Repositories;
public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Member member)
    {
        await _context.Members.AddAsync(member);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Member>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Member> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Member member)
    {
        throw new NotImplementedException();
    }
}
