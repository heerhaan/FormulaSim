using FormuleCirkelEntity.DAL;
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

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            var updatingObject = await _engines.GetEngineById(id.Value);
            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id, Engine updatedObject)
        {
            if (updatedObject is null) { return NotFound(); }
            updatedObject.Id = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (await _engines.FirstOrDefault(res => res.Id == id) is null)
                return NotFound();

            _engines.Update(updatedObject);
            await _engines.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _engines.GetEngineById(id.Value, true);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await _engines.GetEngineById(id, true);
            if (objectToDelete == null)
                return NotFound();

            _engines.Archive(objectToDelete);
            await _engines.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("Archived")]
        public async Task<IActionResult> ArchivedEngines()
        {
            return View(await _engines.GetArchivedEngines());
        }
    }
}
