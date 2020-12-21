﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Race
    {
        [Key]
        public int RaceId { get; set; }
        public int Round { get; set; }
        public string Name { get; set; }

        [EnumDataType(typeof(RaceState))]
        public RaceState RaceState { get; set; }
        [EnumDataType(typeof(Weather))]
        public Weather Weather { get; set; }

        public int StintProgress { get; set; }

        public int TrackId { get; set; }
        public Track Track { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public IList<Stint> Stints { get; } = new List<Stint>();
        public IList<DriverResult> DriverResults { get; } = new List<DriverResult>();
    }

    public enum RaceState
    {
        Concept = 0,
        Qualifying = 1,
        Race = 2,
        Finished = 3
    }

    public enum Weather
    {
        Sunny = 0,
        Overcast = 1,
        Rain = 2,
        Storm = 3
    }
}
