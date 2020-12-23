using System;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverService : IDataService<Driver>
    {
    }

    public class DriverService : DataService<Driver>, IDriverService
    {
        public DriverService(FormulaContext context) : base(context) { }
    }
}
