using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public enum Status { Finished, DNF, DSQ}
    public class DriverResult
    {
        [Key]
        public int DriverResultId { get; set; }
        public int Position { get; set; }
        public int Grid { get; set; }
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }

        public int SeasonDriverId { get; set; }
        public SeasonDriver SeasonDriver { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }
    }
}
