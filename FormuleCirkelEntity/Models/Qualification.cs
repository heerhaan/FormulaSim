using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Qualification
    {
        [Key]
        public int QualyId { get; set; }
        public int Position { get; set; }

        public ICollection<Races> Races { get; set; }
        public ICollection<Drivers> Drivers { get; set; }
        public ICollection<Teams> Teams { get; set; }
    }
}
