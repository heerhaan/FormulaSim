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
        public static string ToolTipSeasonDriverTeamStatus { get { return "First drivers swaps positions with their second driver if one position behind. Also gives +2/-2 to chassis."; } }
        public static string ToolTipSeasonSettingsYear { get { return "Represents the year this season takes place in"; } }
        public static string ToolTipSeasonSettingsPolePoints { get { return "How many points are given for qualifying in 1st position"; } }
        public static string ToolTipSeasonSettingsPitMin { get { return "The lowest possible value for a pitstop, has to be less than the maximum pitstop value and should also be a negative value"; } }
        public static string ToolTipSeasonSettingsPitMax { get { return "The highest possible value for a pitstop, has to be more than the minimum pitstop value and should also be a negative value"; } }
        public static string ToolTipSeasonSettingsQualyRng { get { return "Maximum RNG value that can be achieved during the qualifications with the lower value being 0"; } }
        public static string ToolTipSeasonSettingsQualyQ2 { get { return "The amount of drivers that get to participate in the 2nd qualifying session"; } }
        public static string ToolTipSeasonSettingsQualyQ3 { get { return "The amount of drivers that get to participate in the 3rd qualifying session"; } }
        public static string ToolTipSeasonSettingsQualyBonus { get { return "How many bonus points each position on the starting grid gets over the last qualified driver"; } }
        public static string ToolTipTeamReliability { get { return "Every reliability roll will be between 0 and 100. If the roll is higher than the drivers reliablity value then it causes a DNF"; } }
        public static string ToolTipTeamSpecification { get { return "Specific values that get added to the chassis when the track matches the specification"; } }
    }
}
