﻿using Microsoft.AspNetCore.Identity;

namespace StaffApp.Domain.Entity.Authentication
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
        public bool? IsManagerTypeRole { get; set; }
    }
}
