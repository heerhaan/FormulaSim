using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RacePreviewModel
    {
        public RacePreviewModel() { }
        public RacePreviewModel(Race race, Track track, List<Trait> traits, List<Strategy> strategies, List<SeasonTeam> favourites)
        {
            Race = race;
            Track = track;
            if (traits != null)
            {
                foreach (var trait in traits) { TrackTraits.Add(trait); }
            }
            if (strategies != null)
            {
                foreach (var strat in strategies) { Strategies.Add(strat); }
            }
            if (favourites != null)
            {
                foreach (var team in favourites) { Favourites.Add(team); }
            }
        }
        public Race Race { get; set; }
        public Track Track { get; set; }
        public List<Trait> TrackTraits { get; } = new List<Trait>();
        public List<Strategy> Strategies { get; } = new List<Strategy>();
        public List<SeasonTeam> Favourites { get; } = new List<SeasonTeam>();
    }
}
