using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SeasonTeam> SeasonTeams { get; set; }
    }
}
