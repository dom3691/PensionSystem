using PensionSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Domain.Entities;

public class Contribution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MemberId { get; set; }
    public ContributionType Type { get; set; }
    public decimal Amount { get; set; }
    public Member Member { get; set; }
    public DateTime ContributionDate { get; set; }
    public bool IsVoluntary { get; set; }
    public bool IsDeleted { get; set; }
}
