namespace FormuleCirkelEntity.Utility
{
    public static class Constants
    {
        // Constants for the main role names
        public static readonly string RoleAdmin = "Admin";
        public static readonly string RoleMember = "Member";
        public static readonly string RoleGuest = "Guest";

        // All strings containing info shown in tooltips in the application
        public static readonly string ToolTipImageBtn = "By clicking this button the table above gets copied and posted underneath as an image";
        public static readonly string ToolTipDefaultSeasonBtn = "Default returns the same season setup and races as the previous season";
        public static readonly string ToolTipDropDriverBtn = "To add driver back to the season, just make any modification to them";
        public static readonly string ToolTipModifyRaceBtn = "Press the cog wheel to modify the stints and what happens each stint";
        public static readonly string ToolTipStintEventDriverSkill = "Applies the skill of the driver plus the style bonus";
        public static readonly string ToolTipStintEventTeamChassis = "Applies the chassis of the team plus various bonusses, like specification";
        public static readonly string ToolTipStintEventEnginePower = "Applies the power of the engine";
        public static readonly string ToolTipStintEventTireBonus = "Gives solid +10 to drivers with soft tires";
        public static readonly string ToolTipStintEventTireWear = "RNG of [-20 to 0] for drivers with soft tires";
        public static readonly string ToolTipStintEventQualyBonus = "Applies the bonus the qualifying position gives";
        public static readonly string ToolTipStintEventReliability = "Two rolls between [0 to 100] for the driver and chassis reliability, if any roll is higher than the reliability value then the driver DNFs";
        public static readonly string ToolTipStintEventLowerRng = "Lowest possible RNG for a stint";
        public static readonly string ToolTipStintEventUpperRng = "Highest possible RNG for a stint";
        public static readonly string ToolTipSeasonDriverRandomRelRoll = "RNG for this roll is between 90 and 98";
        public static readonly string ToolTipSeasonDriverTireType = "Soft tires get +10 when tyre bonus applies in stint but will get [-20 to 0] RNG when tyre wear applies.";
        public static readonly string ToolTipSeasonDriverTeamStatus = "First drivers swaps positions with their second driver if one position behind. Also gives +2/-2 to chassis.";
    }
}
