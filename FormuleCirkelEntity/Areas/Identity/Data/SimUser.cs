using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SimUser class
    public class SimUser : IdentityUser
    {
        public string PrincipalName { get; set; }
        public string Country { get; set; }
    }
}
