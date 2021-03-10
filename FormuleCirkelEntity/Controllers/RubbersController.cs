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
    public class RubbersController : FormulaController
    {
        private readonly IRubberService _rubbers;

        public RubbersController(FormulaContext context,
            UserManager<SimUser> userManager,
            IRubberService dataService)
            : base(context, userManager)
        {
            _rubbers = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var rubbers = await _rubbers.GetRubbers();
            return View(rubbers);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rubber updatedObject)
        {
            if (updatedObject is null) { return NotFound(); }
            if (!ModelState.IsValid)
                return View("Create", updatedObject);

            await _rubbers.Add(updatedObject);
            await _rubbers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var updatingObject = await _rubbers.GetRubberById(id);
            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rubber updatedObject)
        {
            if (updatedObject is null) { return NotFound(); }
            updatedObject.RubberId = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (await _rubbers.FirstOrDefault(res => res.RubberId == id) is null)
                return NotFound();

            _rubbers.Update(updatedObject);
            await _rubbers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _rubbers.GetRubberById(id.Value, true);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await _rubbers.GetRubberById(id, true);
            if (objectToDelete == null)
                return NotFound();

            _rubbers.Archive(objectToDelete);
            await _rubbers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("Archived")]
        public async Task<IActionResult> ArchivedRubbers()
        {
            return View(await _rubbers.GetArchivedRubbers());
        }
    }
}
