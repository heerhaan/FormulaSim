using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Drivers
    {
        [Key]
        public int DriverId { get; set; }
        public int DriverNumber { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public int QualyId { get; set; }
        public Qualification Qualification { get; set; }

        public int ResultId { get; set; }
        public Results Results { get; set; }

        public int DriverDetailId { get; set; }
        public DriverDetails DriverDetails { get; set; }
    }
}
