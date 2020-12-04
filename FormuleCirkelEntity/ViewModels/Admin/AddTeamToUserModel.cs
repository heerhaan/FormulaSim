using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.ViewModels
{
    public class AddTeamToUserModel
    {
        public AddTeamToUserModel() { }
        public AddTeamToUserModel(SimUser simUser, IList<Team> teams, List<Team> otherTeams)
        {
            SimUser = simUser;
            if (teams != null)
            {
                foreach (var team in teams.OrderBy(od => od.Abbreviation))
                    OwnedTeams.Add(team);
            }
            if (otherTeams != null)
            {
                foreach (var otherTeam in otherTeams.OrderBy(od => od.Abbreviation))
                    OtherTeams.Add(otherTeam);
            }
        }

        public SimUser SimUser { get; set; }
        public IList<Team> OwnedTeams { get; } = new List<Team>();
        public IList<Team> OtherTeams { get; } = new List<Team>();
    }
}
