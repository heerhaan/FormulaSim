using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class TeamsController : ViewDataController<Team>
    {
        private readonly ITeamService _teams;
        public TeamsController(FormulaContext context,
            UserManager<SimUser> userManager,
            PagingHelper pagingHelper,
            ITeamService dataService)
            : base(context, userManager, pagingHelper, dataService)
        {
            _teams = dataService;
        }

        [SortResult(nameof(Team.Abbreviation)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            ViewBag.teamIds = await Context.Teams.Select(t => t.Id).ToListAsync();
            return base.Index().Result;
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            var updatingObject = await _teams.GetTeamById(id.Value);
            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id, Team updatedObject)
        {
            if (updatedObject is null)
                return NotFound();

            updatedObject.Id = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (await _teams.FirstOrDefault(res => res.Id == id) is null)
                return NotFound();

            _teams.Update(updatedObject);
            await _teams.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _teams.GetTeamById(id.Value, true);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await _teams.GetTeamById(id, true);
            if (objectToDelete == null)
                return NotFound();

            _teams.Archive(objectToDelete);
            await _teams.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int id)
        {
            var stats = new TeamStatsModel();

            // Finds the team corresponding to the given id
            var team = await _teams.GetTeamById(id, true);
            // Only take seasons from the championship that is currently in use
            var seasons = Context.Seasons
                .Where(s => s.Championship.ActiveChampionship)
                .ToList();

            // Basic information about team
            stats.TeamId = team.Id;
            stats.TeamShort = team.Abbreviation;
            stats.TeamBio = team.Biography;

            // Tries to find the last season in which this team was used so their possible long name and team colours can be set
            var lastSeasonTeam = Context.SeasonTeams
                .ToList()
                .LastOrDefault(st => st.TeamId == id);

            if (lastSeasonTeam != null)
            {
                stats.TeamLong = lastSeasonTeam.Name;
                stats.TeamColour = lastSeasonTeam.Colour;
                stats.TeamAccent = lastSeasonTeam.Accent;
            }

            // Selects which drivers have driven for the team
            var drivers = Context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonTeam.TeamId == id)
                .Include(sd => sd.Driver)
                .ToList();

            stats.Drivers = drivers
                .Select(d => d.Driver)
                .Distinct()
                .Select(d => d.Name);

            var results = Context.DriverResults
                .Where(dr => dr.SeasonDriver.SeasonTeam.TeamId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .ToList();

            stats.RaceEntries = results.GroupBy(r => r.RaceId).Count();
            stats.TotalCarEntries = results.Count;
            for (int i = 1; i <= 20; i++)
            {
                int positionCount = results.Count(res => res.Position == i && res.Status == Status.Finished);
                stats.PositionList.Add(i);
                stats.ResultList.Add(positionCount);
            }
            stats.Poles = results.Count(r => r.Grid == 1);
            stats.RaceWins = results.Count(r => r.Position == 1);
            stats.SecondFinishes = results.Count(r => r.Position == 2);
            stats.ThirdFinishes = results.Count(r => r.Position == 3);
            stats.AveragePos = Math.Round(results.Where(res => res.Status == Status.Finished).Average(res => res.Position), 2);
            stats.DidNotFinish = results.Count(r => r.Status == Status.DNF || r.Status == Status.DSQ);

            // Calculate the amount of times the drivers from the team has finished inside the points
            int pointCount = 0;
            foreach (var season in seasons)
            {
                var current = results.Where(r => r.SeasonDriver.SeasonId == season.SeasonId);
                var pointsMax = season.PointsPerPosition.Keys.Max();
                pointCount += (current.Count(dr => dr.Position > 3 && dr.Position <= pointsMax));
            }

            // Apply point finishes and subtract others to form outside point finishes
            stats.PointFinishes = pointCount;
            stats.NoPointFinishes = (stats.TotalCarEntries - stats.RaceWins - stats.SecondFinishes - stats.ThirdFinishes - pointCount - stats.DidNotFinish);

            // Calculates the amount of championships a team has won.
            int teamchamps = 0;
            foreach (var season in seasons)
            {
                var teamwinner = Context.SeasonTeams
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

        [Route("Traits/{id}")]
        public async Task<IActionResult> TeamTraits(int id)
        {
            Team team = await _teams.GetTeamById(id);

            List<Trait> teamTraits = await Context.TeamTraits
                .Where(ttr => ttr.TeamId == id)
                .Select(ttr => ttr.Trait)
                .ToListAsync();

            List<Trait> traits = Context.Traits
                .AsNoTracking()
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Team && !teamTraits.Any(ttr => ttr.TraitId == tr.TraitId))
                .OrderBy(tr => tr.Name)
                .ToList();

            var viewmodel = new TeamTraitsModel
            {
                Team = team,
                TeamTraits = teamTraits,
                Traits = traits
            };
            return View(viewmodel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeamTraits(int id, [Bind("TraitId")] int traitId)
        {
            Team team = await _teams.GetTeamById(id);
            Trait trait = await Context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (team is null || trait is null)
                return NotFound();

            TeamTrait newTrait = new TeamTrait { Team = team, Trait = trait };

            DataService.Update(team);
            await Context.AddAsync(newTrait);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(TeamTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{teamId}")]
        public async Task<IActionResult> RemoveTeamTrait(int teamId, int traitId)
        {
            Team team = await Context.Teams
                .Include(te => te.TeamTraits)
                .FirstAsync(te => te.Id == teamId);
            Trait trait = await Context.Traits
                .FirstAsync(tr => tr.TraitId == traitId);

            if (team == null || trait == null)
                return NotFound();

            TeamTrait removetrait = team.TeamTraits
                .First(ttr => ttr.TraitId == traitId);

            Context.Remove(removetrait);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(TeamTraits), new { id = teamId });
        }

        // This view showcases the teams that lead in varied amount of statistics
        [Route("Leaderlists")]
        public IActionResult Leaderlists()
        {
            var teams = Context.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.Race.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver.SeasonTeam.Team)
                .AsEnumerable()
                .GroupBy(st => st.SeasonDriver.SeasonTeam.Team)
                .ToList();

            var seasons = Context.Seasons
                .Where(s => s.Championship.ActiveChampionship && s.State == SeasonState.Finished)
                .Include(s => s.Teams)
                    .ThenInclude(sd => sd.Team)
                .ToList();

            // Puts ever leading team of every finished season in a dictionary and counts how often they had the most points in a season
            Dictionary<Team, int> teamTitles = new Dictionary<Team, int>();
            foreach (var season in seasons)
            {
                var winner = season.Teams
                    .OrderByDescending(sd => sd.Points)
                    .FirstOrDefault()
                    .Team;

                if (teamTitles.ContainsKey(winner))
                    teamTitles[winner] += 1;
                else
                    teamTitles.Add(winner, 1);
            }
            teamTitles = teamTitles.OrderByDescending(res => res.Value).Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Counts how many races a team has entered
            Dictionary<Team, int> teamStarts = teams
                .Select(t => new { t.Key, Sum = t.GroupBy(r => r.RaceId).Count() })
                .AsEnumerable()
                .Select(t => new KeyValuePair<Team, int>(t.Key, t.Sum))
                .OrderByDescending(res => res.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Creates local functions to be given as a parameter to get the sum of the targeted value
            static int winSelect(DriverResult a) => a.Position == 1 ? 1 : 0;
            static int podiumSelect(DriverResult a) => a.Position <= 3 ? 1 : 0;
            static int nonFinishSelect(DriverResult a) => a.Status == Status.DNF || a.Status == Status.DSQ ? 1 : 0;
            static int poleSelect(DriverResult a) => a.Grid == 1 ? 1 : 0;

            // All the gathered dictionaries are put in a viewmodel and sent over to the view to be put in leaderlists
            TeamLeaderlistsModel viewmodel = new TeamLeaderlistsModel
            {
                LeaderlistTitles = teamTitles,
                LeaderlistWins = GetTeamLeaderlistDict(teams, winSelect),
                LeaderlistPodiums = GetTeamLeaderlistDict(teams, podiumSelect),
                LeaderlistStarts = teamStarts,
                LeaderlistNonFinishes = GetTeamLeaderlistDict(teams, nonFinishSelect),
                LeaderlistPoles = GetTeamLeaderlistDict(teams, poleSelect)
            };

            return View(viewmodel);
        }

        // Generic helper method to get the right dictionary with the given selector
        private static Dictionary<Team, int> GetTeamLeaderlistDict(List<IGrouping<Team, DriverResult>> teams, Func<DriverResult, int> selector)
        {
            Dictionary<Team, int> teamDict = teams
                .Select(t => new { t.Key, Sum = t.Sum(selector) })
                .AsEnumerable()
                .Select(t => new KeyValuePair<Team, int>(t.Key, t.Sum))
                .OrderByDescending(res => res.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return teamDict;
        }

        [Route("Archived")]
        public IActionResult ArchivedTeams()
        {
            var teams = Context.Teams
                .IgnoreQueryFilters()
                .Where(t => t.Archived)
                .OrderBy(t => t.Abbreviation)
                .ToList();

            return View(teams);
        }
        
        [HttpPost("SaveBiography")]
        public IActionResult SaveBiography(int id, string biography)
        {
            var team = Context.Teams.SingleOrDefault(t => t.Id == id);
            team.Biography = biography;
            Context.Teams.Update(team);
            Context.SaveChanges();
            return RedirectToAction("Stats", new { id });
        }
    }
}
