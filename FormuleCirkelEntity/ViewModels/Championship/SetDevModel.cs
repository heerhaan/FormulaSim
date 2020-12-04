using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class SetDevModel
    {
        public SetDevModel() { }
        public SetDevModel(Championship championship)
        {
            if (championship is null) { throw new NullReferenceException(); }

            ChampionshipId = championship.ChampionshipId;
            ChampionshipName = championship.ChampionshipName;
            foreach (var skilldev in championship.SkillDevRanges)
            {
                SkillValueKey.Add(skilldev.ValueKey);
                SkillMinDev.Add(skilldev.MinDev);
                SkillMaxDev.Add(skilldev.MaxDev);
            }
            foreach (var agedev in championship.AgeDevRanges)
            {
                AgeValueKey.Add(agedev.ValueKey);
                AgeMinDev.Add(agedev.MinDev);
                AgeMaxDev.Add(agedev.MaxDev);
            }
        }

        public int ChampionshipId { get; set; }
        public string ChampionshipName { get; set; }
        public IList<int> SkillValueKey { get; } = new List<int>();
        public IList<int> SkillMinDev { get; } = new List<int>();
        public IList<int> SkillMaxDev { get; } = new List<int>();
        public IList<int> AgeValueKey { get; } = new List<int>();
        public IList<int> AgeMinDev { get; } = new List<int>();
        public IList<int> AgeMaxDev { get; } = new List<int>();
    }
}
