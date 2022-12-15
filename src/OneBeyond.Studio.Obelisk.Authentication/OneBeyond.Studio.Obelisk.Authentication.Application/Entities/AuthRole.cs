using System;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.Entities;

public class AuthRole : Microsoft.AspNetCore.Identity.IdentityRole<string>
{
    public AuthRole() : base()
    {
        Id = Guid.NewGuid().ToString();
    }
    public AuthRole(string roleName) : this()
    {
        Name = roleName;
    }

}
