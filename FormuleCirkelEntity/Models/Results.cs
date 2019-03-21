using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Results
    {
        [Key]
        public int ResultId { get; set; }
        public int Grid { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }

        public ICollection<Races> Races { get; set; }
        public ICollection<Drivers> Drivers { get; set; }
        public ICollection<Teams> Teams { get; set; }
        public ICollection<Status> Statuses { get; set; }
    }
}
