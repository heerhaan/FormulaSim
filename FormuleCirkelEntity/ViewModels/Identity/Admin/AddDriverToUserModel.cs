using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class AddDriverToUserModel
    {
        public SimUser SimUser { get; set; }
        public IEnumerable<Driver> OwnedDrivers { get; set; }
        public IEnumerable<Driver> OtherDrivers { get; set; }
    }
}
