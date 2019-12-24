using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Helpers
{
    public static class Utility
    {
        public static readonly Random rng = new Random();

        // Here should methods be placed that have some utility function, like RNG-ing a DNFcause.
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
    }
}
