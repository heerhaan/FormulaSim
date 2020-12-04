using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.ViewModels
{
    public class AddDriverToUserModel
    {
        public AddDriverToUserModel()
        {

        }

        public AddDriverToUserModel(SimUser simUser, IList<Driver> drivers, List<Driver> otherDrivers)
        {
            SimUser = simUser;
            if (drivers != null)
            {
                foreach (var driver in drivers.OrderBy(od => od.Name))
                    OwnedDrivers.Add(driver);
            }
            if (otherDrivers != null)
            {
                foreach (var otherDriver in otherDrivers.OrderBy(od => od.Name))
                    OtherDrivers.Add(otherDriver);
            }
        }
        public SimUser SimUser { get; set; }
        public IList<Driver> OwnedDrivers { get; } = new List<Driver>();
        public IList<Driver> OtherDrivers { get; } = new List<Driver>();
    }
}
