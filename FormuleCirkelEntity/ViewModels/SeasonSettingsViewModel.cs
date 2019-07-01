using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonSettingsViewModel
    {
        public SeasonSettingsViewModel(Season season)
        {
            SeasonId = season.SeasonId;
            QualificationRemainingDriversQ2 = season.QualificationRemainingDriversQ2;
            QualificationRemainingDriversQ3 = season.QualificationRemainingDriversQ3;
        }

        public SeasonSettingsViewModel()
        { }

        public int SeasonId { get; set; }
        public int QualificationRemainingDriversQ2 { get; set; }
        public int QualificationRemainingDriversQ3 { get; set; }
    }
}
