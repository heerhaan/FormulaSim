using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.Models
{
    public class Driver : ModelBase, IArchivable
    {
        public int DriverNumber { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string Biography { get; set; }
        public bool Archived { get; set; }

        public IList<DriverTrait> DriverTraits { get; } = new List<DriverTrait>();
        public IList<SeasonDriver> SeasonDrivers { get; } = new List<SeasonDriver>();

        public int? SimUserId { get; set; }
        public SimUser SimUser { get; set; }
    }
}
