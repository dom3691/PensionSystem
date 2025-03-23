using PensionSystem.Domain.Enums;

namespace PensionSystem.Application.DTOs;

public class ContributionDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public ContributionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContributionDate { get; set; }
    public bool IsVoluntary { get; set; }
}
