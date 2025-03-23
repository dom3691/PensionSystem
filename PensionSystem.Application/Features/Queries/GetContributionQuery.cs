using MediatR;
using PensionSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries;

public class GetContributionQuery : IRequest<ContributionDto>
{
    public Guid Id { get; set; }

}
