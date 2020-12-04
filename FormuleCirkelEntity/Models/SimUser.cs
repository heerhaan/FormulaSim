using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Models
{
    // Add profile data for application users by adding properties to the SimUser class
    public class SimUser : IdentityUser
    {
        public DateTime LastLogin { get; set; }
        public string Country { get; set; }

        public IList<Driver> Drivers { get; } = new List<Driver>();
        public IList<Team> Teams { get; } = new List<Team>();
    }
}
