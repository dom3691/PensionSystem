using PensionSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOs;

public class MemberDto
{
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public static implicit operator MemberDto(Member v)
    {
        throw new NotImplementedException();
    }
}
