using FormuleCirkelEntity.Models;
using GuardNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Builders
{
    public class RaceBuilder
    {
        Race _race;

        public RaceBuilder()
        {
            _race = new Race();
        }

        public RaceBuilder Use(Race race)
        {
            _race = race;
            return this;
        }

        public RaceBuilder InitializeRace(Track track, Season season)
        {
            Guard.NotNull(track, nameof(track));
            Guard.NotNull(season, nameof(season));

            _race.Name = track.Name;
            _race.Track = track;
            _race.Season = season;
            _race.Round = season.Races.Count + 1;
            return this;
        }

        public RaceBuilder AddDefaultStints()
        {
            return AddDefaultStartStint()
                .AddDefaultPitstopStint()
                .AddDefaultCloseStint();
        }

        public RaceBuilder AddModifiedStints(IList<Stint> settings)
        {
            // Pitstop to add before every stint
            Stint pitstop = new Stint()
            {
                RNGMinimum = -7,
                RNGMaximum = -3
            };

            bool firstloop = true;
            foreach (var stint in settings)
            {
                if (firstloop == false)
                {
                    _race.Stints.Add(_race.Stints.Count + 1, pitstop);
                }
                _race.Stints.Add(_race.Stints.Count + 1, stint);
                firstloop = false;
            }
            return this;
        }

        public RaceBuilder AddDefaultStartStint()
        {
            Stint stint = new Stint()
            {
                ApplyChassisLevel = true,
                ApplyDriverLevel = true,
                ApplyEngineLevel = true,
                ApplyQualifyingBonus = true,
                ApplyTireLevel = true,
                RNGMinimum = 0,
                RNGMaximum = 25
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddDefaultPitstopStint()
        {
            Stint stint = new Stint()
            {
                RNGMinimum = -7,
                RNGMaximum = -3
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddDefaultCloseStint()
        {
            Stint stint = new Stint()
            {
                ApplyReliability = true,
                ApplyTireWear = true,
                RNGMinimum = 0,
                RNGMaximum = 35
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddAllDrivers()
        {
            if (_race.Season != null)
                AddDrivers(_race.Season.Drivers);
            return this;
        }

        public RaceBuilder AddDrivers(IEnumerable<SeasonDriver> drivers)
        {
            foreach (var driver in drivers)
                _race.DriverResults.Add(new DriverResult() { SeasonDriver = driver });
            return this;
        }
        
        public void Refresh() => _race = new Race();

        public Race GetResult() => _race;

        public Race GetResultAndRefresh()
        {
            var result = GetResult();
            Refresh();
            return result;
        }
    }
}
