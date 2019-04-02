using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Qualification
    {
        public int? Position { get; set; }
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string DriverName { get; set; }
        public int Score { get; set; }
    }
}
