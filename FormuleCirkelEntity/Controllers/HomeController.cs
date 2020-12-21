﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    public class HomeController : FormulaController
    {
        public HomeController(FormulaContext context, 
            UserManager<SimUser> userManager)
            : base(context, userManager)
        { }

        public IActionResult Index(string message = null)
        {
            bool championship = false;
            string name = "Formula";
            if (_context.Championships.Any())
            {
                championship = true;
                var champname = _context.Championships.FirstOrDefault(c => c.ActiveChampionship);
                name = champname.ChampionshipName;
            }

            HomeViewModel viewModel = new HomeViewModel
            {
                ChampionshipName = name,
                Message = message,
                ChampExists = championship
            };
            return View(viewModel);
        }

        public IActionResult DriverStandings()
        {
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            if (seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                    .Where(s => s.State == SeasonState.Progress)
                    .FirstOrDefault();

                var drivers = _context.SeasonDrivers
                    .IgnoreQueryFilters()
                    .Where(s => s.SeasonId == currentSeason.SeasonId)
                    .Include(s => s.DriverResults)
                    .Include(s => s.Driver)
                    .Include(s => s.SeasonTeam)
                    .OrderByDescending(s => s.Points)
                    .ToList();

                var rounds = _context.Races
                    .Where(r => r.SeasonId == currentSeason.SeasonId)
                    .Include(r => r.Track)
                    .OrderBy(r => r.Round)
                    .ToList();

                List<Track> tracks = new List<Track>();
                foreach (var round in rounds)
                {
                    tracks.Add(round.Track);
                }

                // Calculates the average finishing position
                IList<double> averages = new List<double>();
                foreach (var driver in drivers)
                {
                    List<double> positions = new List<double>();
                    double average = 0;
                    if (driver.DriverResults.Any())
                    {
                        foreach (var result in driver.DriverResults)
                        {
                            if (result.Status == Status.Finished)
                            {
                                positions.Add(result.Position);
                            }
                        }
                        if (positions.Any())
                        {
                            average = Math.Round((positions.Average()), 2);
                        }
                    }
                    averages.Add(average);
                }

                var viewmodel = new HomeDriverStandingsModel
                {
                    SeasonDrivers = drivers,
                    Averages = averages,
                    Tracks = tracks,
                    Rounds = rounds.Select(r => r.RaceId),
                    SeasonId = currentSeason.SeasonId,
                    Year = currentSeason.SeasonNumber,
                    LastPointPos = currentSeason.PointsPerPosition.Keys.Max(),
                    Points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition),
                    PolePoints = currentSeason.PolePoints
                };

                return View(viewmodel);
            } 
            else
            {
                return RedirectToAction("Index", "Season");
            }
        }

        [ActionName("PastDriverStandings")]
        public IActionResult DriverStandings(int seasonId)
        {
            var currentSeason = _context.Seasons
                .Where(s => s.SeasonId == seasonId)
                .FirstOrDefault();

            var drivers = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(s => s.SeasonId == currentSeason.SeasonId)
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)
                .OrderByDescending(s => s.Points)
                .ToList();

            var rounds = _context.Races
                .Where(r => r.SeasonId == currentSeason.SeasonId)
                .Include(r => r.Track)
                .OrderBy(r => r.Round)
                .ToList();

            List<Track> tracks = new List<Track>();
            foreach (var round in rounds)
            {
                tracks.Add(round.Track);
            }

            // Calculates the average finishing position
            IList<double> averages = new List<double>();
            foreach (var driver in drivers)
            {
                List<double> positions = new List<double>();
                double average = 0;
                if (driver.DriverResults.Any())
                {
                    foreach (var result in driver.DriverResults)
                    {
                        if (result.Status == Status.Finished)
                        {
                            positions.Add(result.Position);
                        }
                    }
                    if (positions.Any())
                    {
                        average = Math.Round((positions.Average()), 2);
                    }
                }
                averages.Add(average);
            }

            var viewmodel = new HomeDriverStandingsModel
            {
                SeasonDrivers = drivers,
                Averages = averages,
                Tracks = tracks,
                Rounds = rounds.Select(r => r.RaceId),
                SeasonId = currentSeason.SeasonId,
                Year = currentSeason.SeasonNumber,
                LastPointPos = currentSeason.PointsPerPosition.Keys.Max(),
                Points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition),
                PolePoints = currentSeason.PolePoints
            };

            return View("DriverStandings", viewmodel);
        }

        // Calculates the average finishing position for the given season per driver
        private List<double> CalculateAverageFinish(List<SeasonDriver> drivers)
        {
            List<double> averages = new List<double>();
            foreach (var driver in drivers)
            {
                List<double> positions = new List<double>();
                double average = 0;
                if (driver.DriverResults.Any())
                {
                    foreach (var result in driver.DriverResults)
                    {
                        if (result.Status == Status.Finished)
                        {
                            positions.Add(result.Position);
                        }
                    }
                    if (positions.Any())
                    {
                        average = Math.Round((positions.Average()), 2);
                    }
                }
                averages.Add(average);
            }
            return averages;
        }

        [HttpPost("[Controller]/{seasonId}/GetDriverGraphData")]
        public IActionResult GetDriverGraphData(int seasonId)
        {
            var standings = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonId == seasonId)
                .OrderByDescending(sd => sd.Points)
                .Take(10)
                .Include(sd => sd.Driver)
                .Include(sd => sd.DriverResults)
                .Include(sd => sd.SeasonTeam)
                .ToList();

            return new JsonResult(standings, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public IActionResult TeamStandings()
        {
            // Selects seasons from the currently activated championship
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship).ToList();

            // Checks if there is any season is in progress, else return to season list
            if (seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                    .Where(s => s.State == SeasonState.Progress)
                    .FirstOrDefault();

                var rounds = _context.Races
                    .Where(r => r.SeasonId == currentSeason.SeasonId)
                    .Include(r => r.Track)
                    .OrderBy(r => r.Round)
                    .ToList();

                List<Track> tracks = new List<Track>();
                foreach (var round in rounds)
                {
                    tracks.Add(round.Track);
                }

                var viewModel = new HomeTeamStandingsModel
                {
                    // Assigns the lowest position that scores points
                    LastPointPos = currentSeason.PointsPerPosition.Keys.Max(),
                    SeasonTeams = _context.SeasonTeams
                        .IgnoreQueryFilters()
                        .Include(st => st.Team)
                        .Include(st => st.SeasonDrivers)
                        .Where(st => st.SeasonId == currentSeason.SeasonId)
                        .OrderByDescending(st => st.Points)
                        .ToList(),
                    Tracks = tracks,
                    Rounds = rounds.Select(r => r.RaceId),
                    DriverResults = _context.DriverResults
                        .Where(dr => dr.Race.SeasonId == currentSeason.SeasonId)
                        .ToList(),
                    SeasonId = currentSeason.SeasonId,
                    Year = currentSeason.SeasonNumber,
                    Points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition),
                    PolePoints = currentSeason.PolePoints
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
            
        }

        [ActionName("PastTeamStandings")]
        public IActionResult TeamStandings(int seasonId)
        {
            var currentSeason = _context.Seasons.SingleOrDefault(s => s.SeasonId == seasonId);
            var rounds = _context.Races
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId)
                .Include(r => r.Track)
                .OrderBy(r => r.Round)
                .ToList();

            List<Track> tracks = new List<Track>();
            foreach (var round in rounds)
            {
                tracks.Add(round.Track);
            }

            var viewModel = new HomeTeamStandingsModel
            {
                // Assigns the lowest position that scores points
                LastPointPos = currentSeason.PointsPerPosition.Keys.Max(),
                SeasonTeams = _context.SeasonTeams
                    .IgnoreQueryFilters()
                    .Include(st => st.Team)
                    .Include(st => st.SeasonDrivers)
                    .Where(st => st.SeasonId == seasonId)
                    .OrderByDescending(st => st.Points)
                    .ToList(),
                Tracks = tracks,
                Rounds = rounds.Select(r => r.RaceId),
                DriverResults = _context.DriverResults
                    .Where(dr => dr.Race.SeasonId == seasonId)
                    .ToList(),
                SeasonId = currentSeason.SeasonId,
                Year = currentSeason.SeasonNumber,
                Points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition),
                PolePoints = currentSeason.PolePoints
            };

            return View("TeamStandings", viewModel);
        }

        [HttpPost("[Controller]/{seasonId}/GetTeamGraphData")]
        public IActionResult GetTeamGraphData(int seasonId)
        {
            var season = _context.Seasons.SingleOrDefault(s => s.SeasonId == seasonId);
            if (season == null)
                return null;

            var graphData = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Where(st => st.SeasonId == seasonId)
                .Include(st => st.SeasonDrivers)
                    .ThenInclude(sd => sd.DriverResults)
                .OrderBy(st => st.Name)
                .ToList();

            return new JsonResult(graphData, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public IActionResult NextRace()
        {
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            if(seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                .Where(s => s.State == SeasonState.Progress)
                .Include(r => r.Races)
                .FirstOrDefault();

                var nextrace = currentSeason.Races
                    .OrderBy(r => r.Round)
                    .FirstOrDefault(r => r.RaceState != RaceState.Finished);

                if (nextrace == null)
                {
                    return RedirectToAction("Index", "Season");
                }
                else
                {
                    return RedirectToAction("RacePreview", "Races", new { id = currentSeason.SeasonId, raceId = nextrace.RaceId });
                }
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
        }

        public IActionResult AboutMe()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
