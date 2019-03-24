using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Engine
    {
        [Key]
        public int EngineId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SeasonEngine> SeasonEngines { get; set; }
    }
}
