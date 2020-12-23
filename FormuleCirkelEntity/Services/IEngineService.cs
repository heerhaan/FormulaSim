using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface IEngineService : IDataService<Engine>
    {
    }

    public class EngineService : DataService<Engine>, IEngineService
    {
        public EngineService(FormulaContext context) : base(context) { }
    }
}
