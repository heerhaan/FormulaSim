using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public enum Specification { Topspeed, Acceleration, Handling }

    public class Track : ModelBase, IArchivable
    {
        public Track()
        {
            Traits = new Dictionary<int, Trait>();
        }

        public string Name { get; set; }
        public string Location { get; set; }
        public decimal LengthKM { get; set; }
        public string Country { get; set; }

        [EnumDataType(typeof(Specification))]
        public Specification Specification { get; set; }
        public bool Archived { get; set; }

        public IDictionary<int, Trait> Traits { get; set; }
        public virtual ICollection<Race> Races { get; set; }
    }
}
