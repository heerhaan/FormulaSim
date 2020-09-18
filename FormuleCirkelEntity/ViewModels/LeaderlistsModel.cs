using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class LeaderlistsModel
    {
        public IEnumerable<LeaderlistWin> LeaderlistWins { get; set; }
        public IEnumerable<LeaderlistPodium> LeaderlistPodiums { get; set; }
        // public IEnumerable<LeaderlistPoint> LeaderlistPoints { get; set; }
        public IEnumerable<LeaderlistStart> LeaderlistStarts { get; set; }
        public IEnumerable<LeaderlistNonFinish> LeaderlistNonFinishes { get; set; }
        public IEnumerable<LeaderlistPole> LeaderlistPoles { get; set; }
    }

    public class LeaderlistWin
    {
        public Driver Driver { get; set; }
        public int WinCount { get; set; }
    }

    public class LeaderlistPodium
    {
        public Driver Driver { get; set; }
        public int PodiumCount { get; set; }
    }
    public class LeaderlistPoint
    {
        public Driver Driver { get; set; }
        public int PointsCount { get; set; }
    }

    public class LeaderlistStart
    {
        public Driver Driver { get; set; }
        public int StartCount { get; set; }
    }

    public class LeaderlistNonFinish
    {
        public Driver Driver { get; set; }
        public int NonFinishCount { get; set; }
    }

    public class LeaderlistPole
    {
        public Driver Driver { get; set; }
        public int PoleCount { get; set; }
    }
}
