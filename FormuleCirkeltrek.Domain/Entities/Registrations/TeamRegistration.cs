using FormuleCirkeltrek.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Domain.Entities.Registrations
{
    public class TeamRegistration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Biography { get; set; }

        public Color Color { get; set; }
        public Color Accent { get; set; }
    }
}
