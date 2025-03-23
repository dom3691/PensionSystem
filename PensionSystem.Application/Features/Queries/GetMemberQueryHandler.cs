using MediatR;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOs;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries;

public class GetMemberQueryHandler : IRequestHandler<GetMemberQuery, MemberDto>
{
    private readonly AppDbContext _context;

    public GetMemberQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MemberDto> Handle(GetMemberQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the member by their Id
        var member = await _context.Members
            .Where(m => m.Id == request.Id && !m.IsDeleted)  // Ensure the member is not soft-deleted
            .FirstOrDefaultAsync(cancellationToken);

        // If the member is not found, throw a NotFoundException
        if (member == null)
        {
            throw new BusinessException("Member not found");
        }

        // Map the Member entity to MemberDto
        return new MemberDto
        {
           // Id = member.Id,
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            Phone = member.Phone
        };
    }
}
