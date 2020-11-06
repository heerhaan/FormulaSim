using FormuleCirkelEntity.Areas.Identity.Data;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Data;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Authorization;
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
        public EnginesController(FormulaContext context, IdentityContext identityContext, IAuthorizationService authorizationService, UserManager<SimUser> userManager, PagingHelper pagingHelper)
            : base(context, identityContext, authorizationService, userManager, pagingHelper)
        { }

        [SortResult(nameof(Engine.Name)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            ViewBag.userid = await IsUserOwner();
            return base.Index().Result;
        }

        [Route("Archived")]
        public IActionResult ArchivedEngines()
        {
            List<Engine> engines = Data.IgnoreQueryFilters()
                .Where(e => e.Archived)
                .OrderBy(e => e.Name)
                .ToList();

            return View(engines);
        }
    }
}
