namespace FormuleCirkelEntity.Models
{
    public class AppConfig
    {
        public int Id { get; set; }
        public int DisqualifyChance { get; set; } // Default: 4
        public int MistakeLowerValue { get; set; } // Default: -30
        public int MistakeUpperValue { get; set; } // Default: -15
        // Applies to the upper RNG value of a stint
        public int RainAdditionalRNG { get; set; } // Default: 10
        public int StormAdditionalRNG { get; set; } // Default: 20
        public double SunnyEngineMultiplier { get; set; } // Default: 0.9
        public double OvercastEngineMultiplier { get; set; } // Default: 1.1
        public double WetEngineMultiplier { get; set; } // Default: 1
        public int RainDriverReliabilityModifier { get; set; } // Default: -3
        public int StormDriverReliabilityModifier { get; set; } // Default: -5
        // MistakeAmountRolls refers to the consequent rolls that need to be done before a driver makes a mistake
        // Additional note: mistakes are accounted for in each stint for each driver
        public int MistakeAmountRolls { get; set; } // Default: 2
        // The additional chassis value a #1 driver gets and a #2 driver loses
        public int ChassisModifierDriverStatus { get; set; } // Default: 2
    }
}
