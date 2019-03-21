using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Teams
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }

        public int QualyId { get; set; }
        public Qualification Qualification { get; set; }

        public int ResultId { get; set; }
        public Results Results { get; set; }

        public int TeamDetailId { get; set; }
        public TeamDetails TeamDetails { get; set; }
    }
}
