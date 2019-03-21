using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Engines
    {
        [Key]
        public int EngineId { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        
        public int EngineDetailId { get; set; }
        public EngineDetails EngineDetails { get; set; }

        public int TeamDetailId { get; set; }
        public TeamDetails TeamDetails { get; set; }
    }
}
