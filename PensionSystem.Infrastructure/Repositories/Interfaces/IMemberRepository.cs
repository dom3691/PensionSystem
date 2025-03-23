using PensionSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Infrastructure.Repositories;

public interface IMemberRepository
{
    Task<Member> GetByIdAsync(Guid id);
    Task<IEnumerable<Member>> GetAllAsync();
    Task AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(Guid id);
}
