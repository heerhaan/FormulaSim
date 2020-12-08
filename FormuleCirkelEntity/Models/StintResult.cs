using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class StintResult
    {
        [Key]
        public int StintResultId { get; set; }
        public int Number { get; set; }
        public int Position { get; set; }
        public int Result { get; set; }
        [EnumDataType(typeof(StintStatus))]
        public StintStatus StintStatus { get; set; }

        public int DriverResultId { get; set; }
        public DriverResult DriverResult { get; set; }
    }

    public enum StintStatus
    {
        Concept = 0,
        Running = 1,
        DriverDNF = 2,
        ChassisDNF = 3
    }
}
