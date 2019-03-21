using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public enum DrivingStyle { Aggresive, Neutral, Defensive }
    public enum Tires { Softs, Hards}

    public class DriverDetails
    {
        [Key]
        public int DriverDetailId { get; set; }
        public int Skill { get; set; }
        [EnumDataType(typeof(DrivingStyle))]
        public DrivingStyle DrivingStyle { get; set; }
        [EnumDataType(typeof(Tires))]
        public Tires Tires { get; set; }
        

        public int SeasonId { get; set; }
        public Seasons Seasons { get; set; }

        public ICollection<Drivers> Drivers { get; set; }
    }
}
