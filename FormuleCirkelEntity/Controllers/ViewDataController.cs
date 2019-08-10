using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
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

        public virtual async Task<IActionResult> Index([FromQuery] int? page = 1, [FromQuery] int? givenPageSize = 10)
        {
            var pageSize = PagingHelper.CheckPageSize(givenPageSize ?? 10);
            int pageNumber = page ?? 1;

            int count = await DataContext.Set<T>().CountAsync();

            IQueryable<T> drivers = Data.Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

            ViewBag.pageCount = PagingHelper.GetPageCount(pageSize, count);
            return await AsTask(View(drivers));
        }

        public virtual async Task<IActionResult> Create()
        {
            var newObject = new T();
            return await AsTask(View("Modify", newObject));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T newObject)
        {
            newObject.Id = default(int);
            if (!ModelState.IsValid)
                return View("Modify", newObject);
            DataContext.Add(newObject);
            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public virtual async Task<IActionResult> Edit(int? id)
        {
            T updatingObject = await Data.FindAsync(id);

            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, T updatedObject)
        {
            updatedObject.Id = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (!await Data.AnyAsync(obj => obj.Id == id))
                return NotFound();

            DataContext.Update(updatedObject);
            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);

            if (team == null)
                return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await Data.IgnoreQueryFilters().FindAsync(id);
            if (objectToDelete == null)
                return NotFound();

            if (objectToDelete is IArchivable archivable && archivable.Archived)
                DataContext.Restore(archivable);
            else
                DataContext.Remove(objectToDelete);

            await DataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
