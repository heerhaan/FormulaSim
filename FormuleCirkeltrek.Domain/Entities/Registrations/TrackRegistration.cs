using FormuleCirkeltrek.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Domain.Entities.Registrations
{
    public class TrackRegistration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public Length Length { get; set; }
    }
}
