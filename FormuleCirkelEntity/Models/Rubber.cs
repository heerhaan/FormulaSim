using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Rubber : IArchivable
    {
        public int RubberId { get; set; }
        public string Name { get; set; }
        public string Colour { get; set; }
        public string Accent { get; set; }
        public int PaceMod { get; set; }
        public int MaxWearMod { get; set; }
        public int MinWearMod { get; set; }
        public bool Archived { get; set; }
    }
}
