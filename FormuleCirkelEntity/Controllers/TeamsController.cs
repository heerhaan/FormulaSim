using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class TeamsController : ViewDataController<Team>
    {
        public TeamsController(FormulaContext context, PagingHelper pagingHelper)
            : base(context, pagingHelper)
        { }

        [SortResult(nameof(Team.Abbreviation)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return base.Index();
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            var stats = new TeamStatsModel();

            // Prepares table items for ViewModel
            var team = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);
            var seasons = DataContext.Seasons
                .Where(s => s.Championship.ActiveChampionship)
                .ToList();

            // Basic information about team
            stats.TeamId = team.Id;
            stats.TeamShort = team.Abbreviation;
            stats.TeamBio = team.Biography;

            // Acquire team colours
            var lastSeasonTeam = DataContext.SeasonTeams
                .Where(st => st.TeamId == id)
                .LastOrDefault();

            if (lastSeasonTeam != null)
            {
                stats.TeamLong = lastSeasonTeam.Name;
                stats.TeamColour = lastSeasonTeam.Colour;
                stats.TeamAccent = lastSeasonTeam.Accent;
            }

            // Selects which drivers have driven for the team
            var drivers = DataContext.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonTeam.TeamId == id)
                .Include(sd => sd.Driver)
                .ToList();

            stats.Drivers = drivers
                .Select(d => d.Driver)
                .Distinct()
                .Select(d => d.Name);

            var results = DataContext.DriverResults
                .Where(dr => dr.SeasonDriver.SeasonTeam.TeamId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .ToList();

            stats.RaceEntries = results.GroupBy(r => r.RaceId).Count();
            stats.TotalCarEntries = results.Count;
            stats.Poles = results.Where(r => r.Grid == 1).Count();
            stats.RaceWins = results.Where(r => r.Position == 1).Count();
            stats.SecondFinishes = results.Where(r => r.Position == 2).Count();
            stats.ThirdFinishes = results.Where(r => r.Position == 3).Count();
            stats.DidNotFinish = results.Where(r => r.Status == Status.DNF || r.Status == Status.DSQ).Count();

            // Calculate point finishes
            int pointCount = 0;
            foreach (var season in seasons)
            {
                var current = results.Where(r => r.SeasonDriver.SeasonId == season.SeasonId);
                var pointsMax = season.PointsPerPosition.Keys.Max();
                pointCount += (current.Where(dr => dr.Position > 3 && dr.Position <= pointsMax).Count());
            }

            // Apply point finishes and subtract others to form outside point finishes
            stats.PointFinishes = pointCount;
            stats.NoPointFinishes = (stats.TotalCarEntries - stats.RaceWins - stats.SecondFinishes - stats.ThirdFinishes - pointCount - stats.DidNotFinish);

            // Calculates the amount of championships a team has won.
            int teamchamps = 0;
            foreach (var season in DataContext.Seasons.ToList())
            {
                var teamwinner = DataContext.SeasonTeams
                    .IgnoreQueryFilters()
                    .Where(s => s.SeasonId == season.SeasonId && s.Season.State == SeasonState.Finished)
                    .OrderByDescending(dr => dr.Points)
                    .FirstOrDefault();

                if (teamwinner != null)
                {
                    if (teamwinner.TeamId == id)
                    {
                        teamchamps++;
                    }
                }
            }

            stats.ConstructorTitles = teamchamps;
            return View(stats);
        }

        [Route("Archived")]
        public IActionResult ArchivedTeams()
        {
            var teams = Data.IgnoreQueryFilters()
                .Where(t => t.Archived)
                .OrderBy(t => t.Abbreviation)
                .ToList();

            return View(teams);
        }
        
        [HttpPost("SaveBiography")]
        public IActionResult SaveBiography(int id, string biography)
        {
            var team = DataContext.Teams.SingleOrDefault(t => t.Id == id);
            team.Biography = biography;
            DataContext.Teams.Update(team);
            DataContext.SaveChanges();
            return RedirectToAction("Stats", new { id });
        }
    }
}
