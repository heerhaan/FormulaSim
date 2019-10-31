using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public abstract class ViewDataController<T> : FormulaController
        where T : ModelBase, new()
    {
        protected FormulaContext DataContext { get; }
        protected PagingHelper PagingHelper { get; }
        protected DbSet<T> Data { get; }

        protected ViewDataController(FormulaContext context, PagingHelper pagingHelper)
        {
            (DataContext, PagingHelper) = (context, pagingHelper);
            Data = DataContext.Set<T>();
        }

        [SortResult, PagedResult]
        public virtual async Task<IActionResult> Index()
        {
            return await AsTask(View(Data));
        }

        [Route("Create")]
        public virtual async Task<IActionResult> Create()
        {
            var newObject = new T();
            return await AsTask(View("Modify", newObject));
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T newObject)
        {
            newObject.Id = default(int);
            if(!ModelState.IsValid)
                return View("Modify", newObject);
            DataContext.Add(newObject);
            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            T updatingObject = await Data.FindAsync(id);

            if(updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

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

            DataContext.Update(updatedObject);
            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await Data.IgnoreQueryFilters().FindAsync(id);
            if(objectToDelete == null)
                return NotFound();

            if(objectToDelete is IArchivable archivable && archivable.Archived)
                DataContext.Restore(archivable);
            else
                DataContext.Remove(objectToDelete);

            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
