using MediatR;
using PensionSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Queries;

public class GetContributionSummaryQuery : IRequest<ContributionSummaryDto>
{
    public Guid MemberId { get; set; }
}
