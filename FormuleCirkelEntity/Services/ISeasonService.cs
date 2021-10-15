using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FormuleCirkelEntity.ViewModels.Season;

namespace FormuleCirkelEntity.Services
{
    public interface ISeasonService : IDataService<Season>
    {
        Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false);
        Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false);
        Task<Season> FindActiveSeason(bool withRaces = false);
        Task<SeasonIndexList[]> GetSeasonIndexListOfChampionship(int championshipID);
    }

    public class SeasonService : DataService<Season>, ISeasonService
    {
        public SeasonService(FormulaContext context) : base(context) { }

        public async Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(activeChamp, res => res.Where(s => s.Championship.ActiveChampionship))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .ToListAsync();
        }

        public async Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(withRace, res => res.Include(s => s.Races))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .FirstOrDefaultAsync(res => res.SeasonId == id);
        }

        public async Task<Season> FindActiveSeason(bool withRaces = false)
        {
            return await Data.AsNoTracking()
                .If(withRaces, res => res.Include(s => s.Races))
                .FirstOrDefaultAsync(res => res.Championship.ActiveChampionship && res.State == SeasonState.Progress);
        }

        public async Task<SeasonIndexList[]> GetSeasonIndexListOfChampionship(int championshipID)
        {
            var seasons = await Data.IgnoreQueryFilters()
                .Where(e => e.ChampionshipId == championshipID)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(s => s.Team)
                .OrderByDescending(s => s.SeasonNumber)
                .ToListAsync();

            var indexList = new LinkedList<SeasonIndexList>();
            foreach (var season in seasons)
            {
                var topDriver = season.Drivers.OrderByDescending(e => e.Points).FirstOrDefault();
                var topTeam = season.Teams.OrderByDescending(e => e.Points).FirstOrDefault();

                var seasonVals = new SeasonIndexList()
                {
                    SeasonID = season.SeasonId,
                    SeasonNumber = season.SeasonNumber,
                    TopDriverName = topDriver.Driver.Name,
                    TopDriverCountry = topDriver.Driver.Country,
                    TopDriverTeamColour = topDriver.SeasonTeam.Colour,
                    TopDriverTeamAccent = topDriver.SeasonTeam.Accent,
                    TopTeamName = topTeam.Name,
                    TopTeamCountry = topTeam.Team.Country,
                    TopTeamColour = topTeam.Colour,
                    TopTeamAccent = topTeam.Accent
                };

                indexList.AddLast(seasonVals);
            }
            return indexList.ToArray();
        }
    }
}
