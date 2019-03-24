using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public enum Status { Finished, DNF, DSQ}
    public class DriverResult
    {
        [Key]
        public int DriverResultId { get; set; }
        public int Position { get; set; }
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }

        public int SeasonDriverId { get; set; }
        public SeasonDriver SeasonDriver { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }

        public virtual Qualification Qualification { get; set; }
    }
}
