using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonSetPointsModel
    {
        public SeasonSetPointsModel()
        {
            Points = new List<int>();
        }
        public int SeasonId { get; set; }
        public int SeasonNumber { get; set; }
        public List<int> Points { get; set; }
    }
}
