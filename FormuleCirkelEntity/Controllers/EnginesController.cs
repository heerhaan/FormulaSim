using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    public class EnginesController : ViewDataController<Engine>
    {
        public EnginesController(FormulaContext context, PagingHelper pagingHelper) : base(context, pagingHelper)
        { }

        public IActionResult ArchivedEngines()
        {
            List<Engine> engines = DataContext.Engines.Where(e => e.Archived).ToList();
            return View(engines);
        }
    }
}
