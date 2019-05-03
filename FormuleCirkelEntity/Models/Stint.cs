namespace FormuleCirkelEntity.Models
{
    public class Stint
    {
        public bool ApplyDriverLevel { get; set; }
        public bool ApplyChassisLevel { get; set; }
        public bool ApplyEngineLevel { get; set; }
        public bool ApplyTireLevel { get; set; }
        public bool ApplyQualifyingBonus { get; set; }
        public bool ApplyTireWear { get; set; }
        public bool ApplyReliability { get; set; }

        public int RNGMaximum { get; set; }
        public int RNGMinimum { get; set; }
    }
}