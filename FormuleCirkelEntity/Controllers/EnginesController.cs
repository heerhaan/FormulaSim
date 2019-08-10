using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class EnginesController : ViewDataController<Engine>
    {
        public EnginesController(FormulaContext context, PagingHelper pagingHelper) : base(context, pagingHelper)
        { }

        [Route("Archived")]
        public IActionResult ArchivedEngines()
        {
            List<Engine> engines = Data.IgnoreQueryFilters().Where(e => e.Archived).ToList();
            return View(engines);
        }
    }
}
