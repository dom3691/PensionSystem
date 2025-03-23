using MediatR;
using PensionSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands;

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, bool>
{
    private readonly AppDbContext _context;
    public DeleteMemberCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.Members.FindAsync(request.Id);
        if (member == null) throw new Exception("Member not found");

        member.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
