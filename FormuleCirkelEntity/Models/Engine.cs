using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FormuleCirkelEntity.Models
{
    public class Engine
    {
        [Key]
        public int EngineId { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public bool Available { get; set; }
    }
}
