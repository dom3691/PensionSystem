using MediatR;
using PensionSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands
{
    public class RegisterMemberCommand : IRequest<Guid>
    {
        public MemberDto Member { get; set; }
    }
}
