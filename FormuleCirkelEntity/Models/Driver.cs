using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }
        public int DriverNumber { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Biography { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<SeasonDriver> SeasonDrivers { get; set; }
    }
}
