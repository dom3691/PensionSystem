using PensionSystem.Domain.Entities;

public class MemberDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    // Implicit conversion from Member to MemberDto
    public static implicit operator MemberDto(Member member)
    {
        if (member == null)
            throw new ArgumentNullException(nameof(member));

        return new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            Email = member.Email,
            Phone = member.Phone
        };
    }
}
