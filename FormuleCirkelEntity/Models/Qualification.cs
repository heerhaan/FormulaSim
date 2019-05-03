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

        public int RaceId { get; set; }
        public int DriverId { get; set; }
        public string TeamName { get; set; }

        [StringLength(7)]
        public string Colour { get; set; }

        [StringLength(7)]
        public string Accent { get; set; }

        public string DriverName { get; set; }
        public int? Score { get; set; }
        public int? Position { get; set; }
    }
}
