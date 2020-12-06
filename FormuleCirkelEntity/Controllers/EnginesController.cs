using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class EnginesController : ViewDataController<Engine>
    {
        public EnginesController(FormulaContext context, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper,
            IDataService<Engine> dataService)
            : base(context, userManager, pagingHelper, dataService)
        { }

        [SortResult(nameof(Engine.Name)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            return base.Index().Result;
        }

        [Route("Archived")]
        public IActionResult ArchivedEngines()
        {
            List<Engine> engines = _context.Engines
                .IgnoreQueryFilters()
                .Where(e => e.Archived)
                .OrderBy(e => e.Name)
                .ToList();

            return View(engines);
        }
    }
}
