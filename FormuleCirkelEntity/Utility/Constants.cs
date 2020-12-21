namespace FormuleCirkelEntity.Utility
{
    public static class Constants
    {
        // Constants for the main role names
        public static readonly string RoleAdmin = "Admin";
        public static readonly string RoleMember = "Member";
        public static readonly string RoleGuest = "Guest";

        // All strings containing info shown in tooltips in the application
        public static string ToolTipImageBtn { get { return "By clicking this button the table above gets copied and posted underneath as an image"; } } 
        public static string ToolTipDefaultSeasonBtn { get { return "Default returns the same season setup and races as the previous season"; } }
        public static string ToolTipDropDriverBtn { get { return "To add driver back to the season, just make any modification to them"; } } 
        public static string ToolTipModifyRaceBtn { get { return "Press the cog wheel to modify the stints and what happens each stint"; } }
        public static string ToolTipStintEventDriverSkill { get { return "Applies the skill of the driver plus the style bonus"; } }
        public static string ToolTipStintEventTeamChassis { get { return "Applies the chassis of the team plus various bonusses, like specification"; } }
        public static string ToolTipStintEventEnginePower { get { return "Applies the power of the engine"; } }
        public static string ToolTipStintEventTireBonus { get { return "Gives solid +10 to drivers with soft tires"; } }
        public static string ToolTipStintEventTireWear { get { return "RNG of [-20 to 0] for drivers with soft tires"; } } 
        public static string ToolTipStintEventQualyBonus { get { return "Applies the bonus the qualifying position gives"; } }
        public static string ToolTipStintEventReliability { get { return "Two rolls between [0 to 100] for the driver and chassis reliability, if any roll is higher than the reliability value then the driver DNFs"; } }
        public static string ToolTipStintEventLowerRng { get { return "Lowest possible RNG for a stint"; } }
        public static string ToolTipStintEventUpperRng { get { return "Highest possible RNG for a stint"; } }
        public static string ToolTipSeasonDriverRandomRelRoll { get { return "RNG for this roll is between 90 and 98"; } }
        public static string ToolTipSeasonDriverTireType { get { return "Soft tires get +10 when tyre bonus applies in stint but will get [-20 to 0] RNG when tyre wear applies."; } } 
        public static string ToolTipSeasonDriverTeamStatus { get { return "First drivers swaps positions with their second driver if one position behind. Also gives +2/-2 to chassis."; } }
    }
}
