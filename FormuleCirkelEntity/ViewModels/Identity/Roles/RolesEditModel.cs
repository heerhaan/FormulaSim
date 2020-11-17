using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class RolesEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<SimUser> Members { get; set; }
        public IEnumerable<SimUser> NonMembers { get; set; }
    }
}
