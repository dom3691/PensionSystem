using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Repositories.Interfaces;

public interface IContributionRepository
{
    Task AddAsync(Contribution contribution);
    Task<IEnumerable<Contribution>> GetByMemberIdAsync(Guid memberId);
}
