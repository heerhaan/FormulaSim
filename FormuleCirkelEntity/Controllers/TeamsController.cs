using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class TeamsController : ViewDataController<Team>
    {
        public TeamsController(FormulaContext context, PagingHelper pagingHelper)
            : base(context, pagingHelper)
        { }

        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await Data.FindAsync(id);

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

        public IActionResult ArchivedTeams()
        {
            var teams = Data.IgnoreQueryFilters().Where(t => t.Archived).OrderBy(t => t.Name).ToList();
            return View(teams);
        }

        [HttpPost]
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
