using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands;

public class SoftDeleteMemberCommand : IRequest<bool>
{
    public Guid Id { get; set; }

}
