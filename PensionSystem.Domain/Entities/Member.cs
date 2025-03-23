using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Domain.Entities;

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
}
