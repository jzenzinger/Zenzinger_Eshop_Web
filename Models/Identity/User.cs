﻿using Microsoft.AspNetCore.Identity;

namespace Zenzinger_Eshop_Web.Models.Entity.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}