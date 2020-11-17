using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public abstract class ViewDataController<T> : FormulaController
        where T : ModelBase, new()
    {
        protected PagingHelper PagingHelper { get; }
        protected DbSet<T> Data { get; }

        protected ViewDataController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper)
            : base(context, identityContext, userManager)
        {
            PagingHelper = pagingHelper;
            Data = _context.Set<T>();
        }

        [SortResult, PagedResult]
        public virtual async Task<IActionResult> Index()
        {
            return await AsTask(View(Data));
        }

        [Authorize(Roles = "Admin")]
        [Route("Create")]
        public virtual async Task<IActionResult> Create()
        {
            var newObject = new T();
            return await AsTask(View("Modify", newObject));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T newObject)
        {
            newObject.Id = default(int);
            if(!ModelState.IsValid)
                return View("Modify", newObject);
            _context.Add(newObject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            T updatingObject = await Data.FindAsync(id);

            if(updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id, T updatedObject)
        {
            updatedObject.Id = id;

            if(!ModelState.IsValid)
                return View("Modify", updatedObject);

            if(!await Data.AnyAsync(obj => obj.Id == id))
                return NotFound();

            _context.Update(updatedObject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
                return NotFound();

            var item = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);

            if(item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await Data.IgnoreQueryFilters().FindAsync(id);
            if(objectToDelete == null)
                return NotFound();

            if(objectToDelete is IArchivable archivable && archivable.Archived)
                _context.Restore(archivable);
            else
                _context.Remove(objectToDelete);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
