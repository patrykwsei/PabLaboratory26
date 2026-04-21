using AppCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.EntityFramework.Entities;

public class CrmUser : IdentityUser, ISystemUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string FullName { get; set; }

    public override required string Email { get; set; }

    public required string Department { get; set; }
    public required int Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    public void Activate()
    {
        if (Status == (int)SystemUserStatus.Inactive)
        {
            Status = (int)SystemUserStatus.Active;
            DeactivatedAt = null;
        }
    }

    public void Deactivate(DateTime now)
    {
        if (Status == (int)SystemUserStatus.Active)
        {
            Status = (int)SystemUserStatus.Inactive;
            DeactivatedAt = now;
        }
    }
}