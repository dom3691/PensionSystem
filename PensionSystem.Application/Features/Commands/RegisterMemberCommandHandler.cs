using MediatR;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands;

public class RegisterMemberCommandHandler : IRequestHandler<RegisterMemberCommand, Guid>
{
    private readonly IMemberRepository _repo;

    public RegisterMemberCommandHandler(IMemberRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
    {
        var member = new Member
        {
            FullName = request.Member.FullName,
            DateOfBirth = request.Member.DateOfBirth,
            Email = request.Member.Email,
            Phone = request.Member.Phone
        };

        await _repo.AddAsync(member);
        return member.Id;
    }

}
