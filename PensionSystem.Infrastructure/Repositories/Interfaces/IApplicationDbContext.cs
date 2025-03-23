using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Infrastructure.Repositories.Interfaces
{
    internal interface IApplicationDbContext
    {
        DbSet<Member> Members { get; set; }
        DbSet<Contribution> Contributions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
