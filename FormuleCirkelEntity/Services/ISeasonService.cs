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
using FormuleCirkelEntity.Builders;
using FormuleCirkelEntity.ViewModels;

namespace FormuleCirkelEntity.Services
{
    public interface ISeasonService : IDataService<Season>
    {
        Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false);
        Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false);
        Task<Season> FindActiveSeason(bool withRaces = false);
        Task<SeasonIndexList[]> GetSeasonIndexListOfChampionship(int championshipID);
        Task<int> CreateCopyOfLastSeason(int championshipID);
    }

    public class SeasonService : DataService<Season>, ISeasonService
    {
        private readonly RaceBuilder _raceBuilder;
        public SeasonService(FormulaContext context, RaceBuilder raceBuilder) : base(context)
        {
            _raceBuilder = raceBuilder;
        }

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

        public async Task<int> CreateCopyOfLastSeason(int championshipID)
        {
            var newSeason = new Season();
            var lastSeason = await Data.AsNoTracking()
                .Where(e => e.ChampionshipId == championshipID)
                .Include(e => e.Championship)
                .OrderByDescending(e => e.SeasonNumber)
                .FirstOrDefaultAsync();

            if (lastSeason != null)
            {
                var lastRaces = await Context.Races
                    .Where(e => e.SeasonId == lastSeason.SeasonId)
                    .Include(e => e.Stints)
                    .ToListAsync();

                foreach(var pos in lastSeason.PointsPerPosition) { newSeason.PointsPerPosition.Add(pos); }
                newSeason.PolePoints = lastSeason.PolePoints;
                newSeason.QualificationRemainingDriversQ2 = lastSeason.QualificationRemainingDriversQ2;
                newSeason.QualificationRemainingDriversQ3 = lastSeason.QualificationRemainingDriversQ3;
                newSeason.QualificationRNG = lastSeason.QualificationRNG;
                newSeason.QualyBonus = lastSeason.QualyBonus;
                newSeason.SeasonNumber = (lastSeason.SeasonNumber + 1);
                newSeason.PitMax = lastSeason.PitMax;
                newSeason.PitMin = lastSeason.PitMin;

                foreach (var race in lastRaces.OrderBy(e => e.Round))
                {
                    var track = Context.Tracks.SingleOrDefaultAsync(e => e.Id == race.TrackId);
                    var stints = new List<Stint>();
                    foreach (var lastStint in race.Stints.OrderBy(e => e.Number))
                    {
                        var stint = new Stint
                        {
                            Number = lastStint.Number,
                            ApplyDriverLevel = lastStint.ApplyDriverLevel,
                            ApplyChassisLevel = lastStint.ApplyChassisLevel,
                            ApplyEngineLevel = lastStint.ApplyEngineLevel,
                            ApplyQualifyingBonus = lastStint.ApplyQualifyingBonus,
                            ApplyReliability = lastStint.ApplyReliability,
                            RNGMaximum = lastStint.RNGMaximum,
                            RNGMinimum = lastStint.RNGMinimum
                        };
                        stints.Add(stint);
                    }
                    var newRace = _raceBuilder
                        .InitializeRace(await track, newSeason)
                        .AddModifiedStints(stints)
                        .GetResultAndRefresh();

                    newSeason.Races.Add(newRace);
                }
            }
            newSeason.ChampionshipId = championshipID;
            await Add(newSeason);
            await SaveChangesAsync();

            return newSeason.SeasonId;
        }

        //public async Task<SeasonDetailModel> GenerateSeasonDetailModel(int seasonID)
        //{

        //}
    }
}
