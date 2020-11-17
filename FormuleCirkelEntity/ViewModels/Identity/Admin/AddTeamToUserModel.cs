using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class AddTeamToUserModel
    {
        public SimUser SimUser { get; set; }
        public IEnumerable<Team> OwnedTeams { get; set; }
        public IEnumerable<Team> OtherTeams { get; set; }
    }
}
