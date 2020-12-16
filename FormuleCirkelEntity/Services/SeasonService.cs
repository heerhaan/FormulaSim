using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public static class SeasonService
    {
        // Default assigned points per position
        public static void AddDefaultPoints(Season season)
        {
            // If we have been given a null object then something went very wrong, so we throw
            if (season is null) throw new NullReferenceException();

            // Underneat the defined base values for points in Formula Simulation
            season.PointsPerPosition.Add(1, 25);
            season.PointsPerPosition.Add(2, 18);
            season.PointsPerPosition.Add(3, 15);
            season.PointsPerPosition.Add(4, 12);
            season.PointsPerPosition.Add(5, 10);
            season.PointsPerPosition.Add(6, 8);
            season.PointsPerPosition.Add(7, 6);
            season.PointsPerPosition.Add(8, 5);
            season.PointsPerPosition.Add(9, 4);
            season.PointsPerPosition.Add(10, 3);
            season.PointsPerPosition.Add(11, 2);
            season.PointsPerPosition.Add(12, 1);
        }
    }
}
