using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;

namespace PensionSystem.Application.Features.Commands;

public class SoftDeleteContributionCommandHandler : IRequestHandler<SoftDeleteContributionCommand, bool>
{
    private readonly AppDbContext _context;

    public SoftDeleteContributionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SoftDeleteContributionCommand request, CancellationToken cancellationToken)
    {
        var contribution = await _context.Contributions
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contribution == null)
            throw new BusinessException("Contribution not found");

        contribution.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
