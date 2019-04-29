using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Qualification
    {
        [Key]
        public int QualyId { get; set; }

        public int RaceId { get; set; }
        public int DriverId { get; set; }
        public string TeamName { get; set; }
        public string DriverName { get; set; }
        public int? Score { get; set; }
        public int? Position { get; set; }
    }
}