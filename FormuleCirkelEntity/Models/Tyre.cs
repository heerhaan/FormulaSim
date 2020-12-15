using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Tyre
    {
        [Key]
        public int Id { get; set; }
        public string TyreName { get; set; }
        [StringLength(7)]
        public string TyreColour { get; set; }
        // How many stints can the tyre last, this is necessary for determining which strategies exist for a race
        public int StintLen { get; set; }
        // The initial value the tyre starts with that gets eventually subtracted by the wear
        public int Pace { get; set; }
        // The maximum possible wear for this tyre
        public int MaxWear { get; set; }
        // The minimum possible wear for this tyre
        public int MinWear { get; set; }

        public IList<TyreStrategy> Strategies { get; } = new List<TyreStrategy>();
    }
}
