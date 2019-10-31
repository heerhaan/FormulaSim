using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FormuleCirkelEntity.Models
{
    public class Engine : ModelBase, IArchivable
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public bool Archived { get; set; }
    }
}
