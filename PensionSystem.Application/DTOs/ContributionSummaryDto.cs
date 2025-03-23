using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOs
{
    public class ContributionSummaryDto
    {
        public decimal TotalContributions { get; set; }
        public List<ContributionDto> Contributions { get; set; }
    }
}
