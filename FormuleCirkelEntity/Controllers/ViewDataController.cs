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
            if (newObject is null) { return NotFound(); }
            newObject.Id = default;
            if(!ModelState.IsValid)
                return View("Modify", newObject);

            await DataService.Add(newObject);
            await DataService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
