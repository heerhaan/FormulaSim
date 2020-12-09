using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Utility
{
    // Here should methods be placed that have some utility function, like RNG-ing a DNFcause.
    public static class Helpers
    {
        public static readonly Random rng = new Random();

        // Returns a randomly calculated weather effect
        public static Weather RandomWeather()
        {
            int random = rng.Next(1, 21);
            Weather weather = Weather.Sunny;

            switch (random)
            {
                case int n when n <= 8:
                    weather = Weather.Sunny;
                    break;
                case int n when n > 8 && n <= 16:
                    weather = Weather.Overcast;
                    break;
                case int n when n > 16 && n <= 19:
                    weather = Weather.Rain;
                    break;
                case 20:
                    weather = Weather.Storm;
                    break;
            }

            return weather;
        }

        // Create a dictionary from a given SeasonTeam object
        public static Dictionary<string, int> CreateTeamSpecDictionary(SeasonTeam team)
        {
            if (team is null)
                return null;

            Dictionary<string, int> teamSpecs = new Dictionary<string, int>
            {
                { "Topspeed", team.Topspeed },
                { "Acceleration", team.Acceleration },
                { "Handling", team.Handling }
            };
            return teamSpecs;
        }

        // Calculates the bonus a chassis gets from a specific track specification
        public static int GetChassisBonus(Dictionary<string, int> teamSpecs, string trackSpec)
        {
            int bonus = 0;
            bonus = (teamSpecs.SingleOrDefault(k => k.Key == trackSpec)).Value;
            return bonus;
        }

        // Calculates the bonus a driver gets for their qualifying position
        public static int GetQualifyingBonus(int qualifyingPosition, int totalDriverCount, int qualyBonus)
        {
            return (totalDriverCount * qualyBonus) - (qualifyingPosition * qualyBonus);
        }

        // Calculates the total power level a driver has
        public static int GetPowerDriver(SeasonDriver driver, int modifiers, string trackspec)
        {
            if (driver is null || driver.SeasonTeam is null)
                return 0;

            int power = 0;
            power += driver.Skill;
            power += driver.SeasonTeam.Chassis;
            power += driver.SeasonTeam.Engine.Power;
            power += ((((int)driver.DriverStatus) * -2) + 2);
            power += modifiers;
            power += GetChassisBonus(CreateTeamSpecDictionary(driver.SeasonTeam), trackspec);
            return power;
        }

        // Determines the sort of cause a DNF a driver had, parameters defines if the cause of the DNF was a driver fault or a chassis fault
        public static DNFCause RandomDriverDNF()
        {
            int random = rng.Next(1, 101);
            DNFCause cause = DNFCause.None;

            switch (random)
            {
                case int n when n <= 16:
                    cause = DNFCause.Damage;
                    break;
                case int n when n > 16 && n <= 44:
                    cause = DNFCause.Collision;
                    break;
                case int n when n > 44 && n <= 92:
                    cause = DNFCause.Accident;
                    break;
                case int n when n > 92:
                    cause = DNFCause.Puncture;
                    break;
            }

            return cause;
        }

        public static DNFCause RandomChassisDNF()
        {
            int random = rng.Next(1, 101);
            DNFCause cause = DNFCause.None;

            switch (random)
            {
                case int n when n <= 48:
                    cause = DNFCause.Engine;
                    break;
                case int n when n > 48 && n <= 78:
                    cause = DNFCause.Electrics;
                    break;
                case int n when n > 78 && n <= 84:
                    cause = DNFCause.Exhaust;
                    break;
                case int n when n > 84 && n <= 86:
                    cause = DNFCause.Clutch;
                    break;
                case int n when n > 86 && n <= 96:
                    cause = DNFCause.Hydraulics;
                    break;
                case int n when n > 96 && n <= 98:
                    cause = DNFCause.Wheel;
                    break;
                case int n when n > 98:
                    cause = DNFCause.Brakes;
                    break;
            }

            return cause;
        }

        // Determines the sort of cause a DSQ a driver had, parameters defines if the cause of the DNF was a driver fault or a chassis fault
        public static DSQCause RandomDriverDSQ()
        {
            return DSQCause.Dangerous;
        }

        public static DSQCause RandomChassisDSQ()
        {
            DSQCause cause = DSQCause.None;
            int random = rng.Next(1, 11);

            if (random < 9)
                cause = DSQCause.Illegal;
            else
                cause = DSQCause.Fuel;

            return cause;
        }
    }
}
