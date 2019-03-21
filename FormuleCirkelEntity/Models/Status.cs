using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusText { get; set; }

        public int ResultId { get; set; }
        public Results Results { get; set; }
    }
}
