using MediatR;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;

namespace PensionSystem.Application.Features.Commands;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly AppDbContext _context;

    public UpdateMemberCommandHandler(AppDbContext context)
    {
        _context = context;

    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.Members.FindAsync(request.Id);
        if (member == null) throw new Exception ("Member not found");

        member.FullName = request.FullName;
        member.DateOfBirth = request.DateOfBirth;
        member.Email = request.Email;
        member.Phone = request.Phone;

        await _context.SaveChangesAsync(cancellationToken);
        return new MemberDto
        {
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            Phone = member.Phone
        };
    }

}
