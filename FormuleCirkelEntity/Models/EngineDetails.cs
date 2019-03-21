using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class EngineDetails
    {
        [Key]
        public int EngineDetailId { get; set; }
        public int Power { get; set; }

        public int SeasonId { get; set; }
        public Seasons Seasons { get; set; }

        public ICollection<Engines> Engines { get; set; }
    }
}
