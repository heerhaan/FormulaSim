using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Domain.Entities
{
    public class Season
    {
        public Guid Id { get; set; }
        public RacingClass Class { get; set; }
    }
}
