using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonSettingsViewModel
    {
        public SeasonSettingsViewModel() 
        {
            QualificationRNG = 40;
            QualyBonus = 2;
            PitMin = -65;
            PitMax = -55;
        }

        public SeasonSettingsViewModel(FormuleCirkelEntity.Models.Season season)
        {
            SeasonId = season.SeasonId;
            SeasonNumber = season.SeasonNumber;
            QualificationRNG = season.QualificationRNG;
            QualificationRemainingDriversQ2 = season.QualificationRemainingDriversQ2;
            QualificationRemainingDriversQ3 = season.QualificationRemainingDriversQ3;
            QualyBonus = season.QualyBonus;
            PolePoints = season.PolePoints;
            PitMin = season.PitMin;
            PitMax = season.PitMax;
        }

        public int SeasonId { get; set; }
        public int SeasonNumber { get; set; }
        public int QualificationRNG { get; set; }
        public int QualificationRemainingDriversQ2 { get; set; }
        public int QualificationRemainingDriversQ3 { get; set; }
        public int QualyBonus { get; set; }
        public int PitMin { get; set; }
        public int PitMax { get; set; }
        public int PolePoints { get; set; }
    }
}
