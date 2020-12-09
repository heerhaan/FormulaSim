using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Stint
    {
        [Key]
        public int StintId { get; set; }
        public int Number { get; set; }
        public bool ApplyPitstop { get; set; }
        public bool ApplyDriverLevel { get; set; }
        public bool ApplyChassisLevel { get; set; }
        public bool ApplyEngineLevel { get; set; }
        public bool ApplyTireLevel { get; set; }
        public bool ApplyQualifyingBonus { get; set; }
        public bool ApplyTireWear { get; set; }
        public bool ApplyReliability { get; set; }
        public int RNGMaximum { get; set; }
        public int RNGMinimum { get; set; }

        public int RaceId { get; set; }
        public Race Race { get; set; }
    }
}