using Microsoft.AspNetCore.Identity;

namespace Zenzinger_Eshop_Web.Models.Entity.Identity
{
    public class Role : IdentityRole<int>
    {
        public Role() : base() { }
        public Role(string role) : base(role) { }
    }
}