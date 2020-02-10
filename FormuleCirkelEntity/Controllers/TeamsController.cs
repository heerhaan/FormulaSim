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

        [SortResult(nameof(Team.Name)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return base.Index();
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);

            var seasondrivers = DataContext.SeasonDrivers
                .Where(sd => sd.SeasonTeam.TeamId == id)
                .Include(sd => sd.Driver);

            var driverresults = DataContext.DriverResults
                .Where(dr => dr.SeasonDriver.SeasonTeam.TeamId == id);

            // Calculates the amount of championships a team has won.
            int driverchamps = 0;
            int teamchamps = 0;
            foreach (var season in DataContext.Seasons)
            {
                var driverwinner = DataContext.SeasonDrivers
                    .Where(s => s.SeasonId == season.SeasonId && s.Season.State == SeasonState.Finished)
                    .OrderByDescending(dr => dr.Points)
                    .FirstOrDefault();

                var teamwinner = DataContext.SeasonTeams
                    .Where(s => s.SeasonId == season.SeasonId && s.Season.State == SeasonState.Finished)
                    .OrderByDescending(dr => dr.Points)
                    .FirstOrDefault();

                if (driverwinner != null)
                {
                    if (seasondrivers.Any(s => s.SeasonDriverId == driverwinner.SeasonDriverId))
                    {
                        driverchamps++;
                    }
                }

                if (teamwinner != null)
                {
                    if (teamwinner.TeamId == id)
                    {
                        teamchamps++;
                    }
                }
            }

            ViewBag.driverchampionships = driverchamps;
            ViewBag.teamchampionships = teamchamps;

            var stats = new TeamStatsModel()
            {
                Team = team,
                SeasonDriver = seasondrivers,
                DriverResults = driverresults
            };

            return View(stats);
        }

        [Route("Archived")]
        public IActionResult ArchivedTeams()
        {
            var teams = Data.IgnoreQueryFilters().Where(t => t.Archived).OrderBy(t => t.Name).ToList();
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
