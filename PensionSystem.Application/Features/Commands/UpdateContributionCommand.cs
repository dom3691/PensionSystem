﻿using MediatR;
using PensionSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Features.Commands;

public class UpdateContributionCommand : IRequest<ContributionDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public bool IsVoluntary { get; set; }
}
