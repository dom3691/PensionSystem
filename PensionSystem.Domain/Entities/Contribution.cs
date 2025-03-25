using PensionSystem.Domain.Enums;

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
    public bool IsValidated { get; set; }
}
