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
        protected IDataService<T> DataService { get; }

        protected ViewDataController(FormulaContext context, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper,
            IDataService<T> dataService)
            : base(context, userManager)
        {
            PagingHelper = pagingHelper;
            DataService = dataService;
        }

        [SortResult, PagedResult]
        public virtual async Task<IActionResult> Index()
        {
            return await AsTask(View(DataService.GetQueryable()));
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

            await DataService.Add(newObject);
            await DataService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            T updatingObject = await DataService.GetEntityById(id.Value);
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

            if(await DataService.FirstOrDefault(res => res.Id == id) is null)
                return NotFound();

            DataService.Update(updatedObject);
            await DataService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
                return NotFound();

            var item = await DataService.GetEntityByIdUnfiltered(id.Value);

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
            var objectToDelete = await DataService.GetEntityByIdUnfiltered(id);
            if(objectToDelete == null)
                return NotFound();

            DataService.Archive(objectToDelete);
            await DataService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
