using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        [StringLength(7)]
        public string Colour { get; set; }
        [StringLength(7)]
        public string Accent { get; set; }

        public virtual ICollection<SeasonTeam> SeasonTeams { get; set; }
    }
}
