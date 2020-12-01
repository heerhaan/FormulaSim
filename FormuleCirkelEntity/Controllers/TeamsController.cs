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
        public TeamsController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper)
            : base(context, identityContext, userManager, pagingHelper)
        { }

        [SortResult(nameof(Team.Abbreviation)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            // Checks if the user is authenticated and sends the list of owned team id's if that's the case
            // Other wise assigns an empty int list to prevent a nullreference in the view
            if (User.Identity.IsAuthenticated)
            {
                SimUser simuser = await _userManager.GetUserAsync(User);
                ViewBag.ownedteams = simuser.Teams;
            }
            else
                ViewBag.ownedteams = new List<int>();

            return base.Index().Result;
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int? id)
        {
            if (id is null)
                return NotFound();

            var stats = new TeamStatsModel();

            // Finds the team corresponding to the given id
            var team = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);
            // Only take seasons from the championship that is currently in use
            var seasons = _context.Seasons
                .Where(s => s.Championship.ActiveChampionship)
                .ToList();

            // Basic information about team
            stats.TeamId = team.Id;
            stats.TeamShort = team.Abbreviation;
            stats.TeamBio = team.Biography;

            // Tries to find the last season in which this team was used so their possible long name and team colours can be set
            var lastSeasonTeam = _context.SeasonTeams
                .ToList()
                .Where(st => st.TeamId == id)
                .LastOrDefault();

            if (lastSeasonTeam != null)
            {
                stats.TeamLong = lastSeasonTeam.Name;
                stats.TeamColour = lastSeasonTeam.Colour;
                stats.TeamAccent = lastSeasonTeam.Accent;
            }

            // Selects which drivers have driven for the team
            var drivers = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonTeam.TeamId == id)
                .Include(sd => sd.Driver)
                .ToList();

            stats.Drivers = drivers
                .Select(d => d.Driver)
                .Distinct()
                .Select(d => d.Name);

            var results = _context.DriverResults
                .Where(dr => dr.SeasonDriver.SeasonTeam.TeamId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .ToList();

            stats.RaceEntries = results.GroupBy(r => r.RaceId).Count();
            stats.TotalCarEntries = results.Count;
            stats.Poles = results.Where(r => r.Grid == 1).Count();
            stats.RaceWins = results.Where(r => r.Position == 1).Count();
            stats.SecondFinishes = results.Where(r => r.Position == 2).Count();
            stats.ThirdFinishes = results.Where(r => r.Position == 3).Count();
            stats.DidNotFinish = results.Where(r => r.Status == Status.DNF || r.Status == Status.DSQ).Count();

            // Calculate the amount of times the drivers from the team has finished inside the points
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
            foreach (var season in seasons)
            {
                var teamwinner = _context.SeasonTeams
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
            Team team = await Data
                .FirstAsync(dr => dr.Id == id);

            List<Trait> teamTraits = await _context.TeamTraits
                .Where(ttr => ttr.TeamId == id)
                .Select(ttr => ttr.Trait)
                .ToListAsync();

            List<Trait> traits = await _context.Traits
                .Where(tr => tr.TraitGroup == TraitGroup.Team && !teamTraits.Any(ttr => ttr.TraitId == tr.TraitId))
                .OrderBy(tr => tr.Name)
                .ToListAsync();

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
            Team team = await Data.FirstAsync(t => t.Id == id);
            Trait trait = await _context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (team is null || trait is null)
                return NotFound();

            TeamTrait newTrait = new TeamTrait { Team = team, Trait = trait };
            await _context.AddAsync(newTrait);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeamTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{teamId}")]
        public async Task<IActionResult> RemoveTeamTrait(int teamId, int traitId)
        {
            Team team = await Data.Include(te => te.TeamTraits).FirstAsync(te => te.Id == teamId);
            Trait trait = await _context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (team == null || trait == null)
                return NotFound();

            TeamTrait removetrait = team.TeamTraits.First(ttr => ttr.TraitId == traitId);
            _context.Remove(removetrait);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeamTraits), new { id = teamId });
        }

        // This view showcases the teams that lead in varied amount of statistics
        [Route("Leaderlists")]
        public IActionResult Leaderlists()
        {
            var teams = _context.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.Race.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver.SeasonTeam.Team)
                .AsEnumerable()
                .GroupBy(st => st.SeasonDriver.SeasonTeam.Team)
                .ToList();

            var seasons = _context.Seasons
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
            var teams = Data.IgnoreQueryFilters()
                .Where(t => t.Archived)
                .OrderBy(t => t.Abbreviation)
                .ToList();

            return View(teams);
        }
        
        [HttpPost("SaveBiography")]
        public IActionResult SaveBiography(int id, string biography)
        {
            var team = _context.Teams.SingleOrDefault(t => t.Id == id);
            team.Biography = biography;
            _context.Teams.Update(team);
            _context.SaveChanges();
            return RedirectToAction("Stats", new { id });
        }
    }
}
