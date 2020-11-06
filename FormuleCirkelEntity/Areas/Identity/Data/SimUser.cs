using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SimUser class
    public class SimUser : IdentityUser
    {
        public SimUser()
        {
            Drivers = new List<int>();
            Teams = new List<int>();
        }
        public string FullName { get; set; }
        public string Country { get; set; }

        public IList<int> Drivers { get; set; }
        public IList<int> Teams { get; set; }
    }
}
