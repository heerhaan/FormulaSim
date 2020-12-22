using System;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverService
    {
    }

    public class DriverService : IDriverService
    {
        private readonly FormulaContext _context;

        public DriverService(FormulaContext context)
        {
            _context = context;
        }
    }
}
