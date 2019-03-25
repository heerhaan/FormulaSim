using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Qualification
    {
        [Key]
        public int QualyId { get; set; }
        public int Position { get; set; }

        public int DriverRef { get; set; }
        public virtual DriverResult DriverResult { get; set; }
    }
}
