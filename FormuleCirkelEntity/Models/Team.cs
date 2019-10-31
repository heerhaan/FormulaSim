using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Team : ModelBase, IArchivable
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        [StringLength(7)]
        public string Colour { get; set; }

        [StringLength(7)]
        public string Accent { get; set; }
        public string Biography { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<SeasonTeam> SeasonTeams { get; set; }
    }
}
