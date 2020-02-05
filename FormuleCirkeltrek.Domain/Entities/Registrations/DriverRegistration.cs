using FormuleCirkeltrek.Domain.ValueObjects;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Domain.Entities.Registrations
{
    public class DriverRegistration
    {
        public DriverRegistration()
        {
            Id = Guid.NewGuid();
            Number = new DriverNumber(01);
            Name = string.Empty;
            Abbreviation = string.Empty;
            DateOfBirth = DateTime.Today;
            Biography = string.Empty;

        }

        public Guid Id { get; set; }
        public DriverNumber Number { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Biography { get; set; }
    }
}
