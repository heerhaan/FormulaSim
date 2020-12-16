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
                .AddDefaultMiddleStint()
                .AddDefaultCloseStint();
        }

        public RaceBuilder AddModifiedStints(IList<Stint> settings)
        {
            if (settings is null) { return null; }
            int stintNr = 0;
            foreach (var stint in settings)
            {
                stint.Number = ++stintNr;
                _race.Stints.Add(stint);
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
                RNGMinimum = 10,
                RNGMaximum = 35
            };

            _race.Stints.Add(stint);
            return this;
        }

        public RaceBuilder AddDefaultMiddleStint()
        {
            Stint stint = new Stint()
            {
                RNGMinimum = 10,
                RNGMaximum = 40
            };

            _race.Stints.Add(stint);
            return this;
        }

        public RaceBuilder AddDefaultCloseStint()
        {
            Stint stint = new Stint()
            {
                ApplyReliability = true,
                RNGMinimum = 10,
                RNGMaximum = 45
            };

            _race.Stints.Add(stint);
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
            // Check if given parameters aren't null
            if (drivers is null)
                throw new NullReferenceException();

            foreach (var driver in drivers)
            {
                // Ensures that dropped drivers won't be added to a race by not creating a result object for them
                if (driver.Dropped)
                    continue;

                DriverResult driverResult = new DriverResult { SeasonDriver = driver };
                foreach (var stint in _race.Stints)
                {
                    StintResult driverStint = new StintResult
                    {
                        StintStatus = StintStatus.Concept,
                        Number = stint.Number
                    };
                    driverResult.StintResults.Add(driverStint);
                }
                _race.DriverResults.Add(driverResult);
            }
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
