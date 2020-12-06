using System;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverService
    {
        Task<Driver> GetDriverByIdAsync(int id);
        IQueryable<Driver> GetDrivers();
        Task<bool> AddDriverAsync(Driver driver);
        Task<bool> ModifyDriverAsync(Driver driver);
        Task<bool> ArchiveDriverAsync(int id);
    }

    public class DriverService : IDriverService
    {
        private readonly FormulaContext _context;

        public DriverService(FormulaContext context)
        {
            _context = context;
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        public IQueryable<Driver> GetDrivers()
        {
            return _context.Drivers;
        }

        public async Task<bool> AddDriverAsync(Driver driver)
        {
            return false;
        }

        public async Task<bool> ModifyDriverAsync(Driver driver)
        {
            return false;
        }

        public async Task<bool> ArchiveDriverAsync(int id)
        {
            return false;
        }
    }
}
