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
        private readonly IEngineService _engines;

        public EnginesController(FormulaContext context,
            UserManager<SimUser> userManager,
            PagingHelper pagingHelper,
            IEngineService dataService)
            : base(context, userManager, pagingHelper, dataService)
        {
            _engines = dataService;
        }

        [SortResult(nameof(Engine.Name)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(base.Index().Result);
        }

        [Route("Archived")]
        public async Task<IActionResult> ArchivedEngines()
        {
            return View(await _engines.GetArchivedEngines().ConfigureAwait(false));
        }
    }
}
