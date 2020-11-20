using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class SetDevModel
    {
        public int ChampionshipId { get; set; }
        public IList<int> SkillKey { get; } = new List<int>();
        public IList<int> SkillLower { get; } = new List<int>();
        public IList<int> SkillHigher { get; } = new List<int>();
        public IList<int> AgeKey { get; } = new List<int>();
        public IList<int> AgeLower { get; } = new List<int>();
        public IList<int> AgeHigher { get; } = new List<int>();
    }
}
