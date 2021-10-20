using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Models;
using Newtonsoft.Json;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    public class FormulaController : Controller
    {
        protected FormulaController(FormulaContext context,
            UserManager<SimUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        protected FormulaContext Context { get; }
        protected UserManager<SimUser> UserManager { get; }

        protected static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        #region datacall prefabs
        /// <summary>
        /// Created a JSON data answers from async calls, returning a list or detail object by ID.
        /// </summary>
        /// <param name="key">The nillable identity value</param>
        /// <param name="listCall">The overview call</param>
        /// <param name="detailCall">Detail call, will be supplied with parameter value if not nil</param>
        /// <returns>A JsON-interpretable contentresult with JsON content-type</returns>
        protected async Task<ContentResult> DataCall<TKey, TOverview, TDetail>(TKey? key, Func<Task<TOverview[]>> listCall, Func<TKey, Task<TDetail>> detailCall)
            where TKey : struct
            => await DataCall<TKey, TOverview, TDetail, object>(key, listCall, detailCall, null);
        /// <summary>
        /// Created a JSON data answers from async calls, returning a list or detail object by ID.
        /// </summary>
        /// <param name="key">The nillable identity value</param>
        /// <param name="listCall">The overview call</param>
        /// <param name="detailCall">Detail call, will be supplied with parameter value if not nil</param>
        /// <param name="orderBy">Basic ordering function</param>
        /// <returns>A JsON-interpretable contentresult with JsON content-type</returns>
        protected async Task<ContentResult> DataCall<TKey, TOverview, TDetail, TSortProperty>(TKey? key, Func<Task<TOverview[]>> listCall, Func<TKey, Task<TDetail>> detailCall, Func<TOverview, TSortProperty> orderBy)
            where TKey : struct
        {
            if (!key.HasValue)
            {
                var data = await listCall();
                if (orderBy == null) { return Json(data); }
                else { return Json(data.OrderBy(orderBy).ToArray()); }
            }
            else
            {
                return Json(await detailCall(key.Value));
            }
        }

        /// <summary>
        /// Created a JSON data answers from async calls, returning a list or detail object by ID.
        /// </summary>
        /// <param name="key">The (string) identity value</param>
        /// <param name="listCall">The overview call</param>
        /// <param name="detailCall">Detail call, will be supplied with parameter value if not nil</param>
        /// <returns>A JsON-interpretable contentresult with JsON content-type</returns>
        protected async Task<ContentResult> DataCall<TOverview, TDetail>(string key, Func<Task<TOverview[]>> listCall, Func<string, Task<TDetail>> detailCall)
            => await DataCall<TOverview, TDetail, object>(key, listCall, detailCall, null);
        /// <summary>
        /// Created a JSON data answers from async calls, returning a list or detail object by ID.
        /// </summary>
        /// <param name="key">The (string) identity value</param>
        /// <param name="listCall">The overview call</param>
        /// <param name="detailCall">Detail call, will be supplied with parameter value if not nil</param>
        /// <param name="orderBy">Basic ordering function</param>
        /// <returns>A JsON-interpretable contentresult with JsON content-type</returns>
        protected async Task<ContentResult> DataCall<TOverview, TDetail, TSortProperty>(string key, Func<Task<TOverview[]>> listCall, Func<string, Task<TDetail>> detailCall, Func<TOverview, TSortProperty> orderBy)
        {
            if (string.IsNullOrEmpty(key))
            {
                var data = await listCall();
                if (orderBy == null) { return Json(data); }
                else { return Json(data.OrderBy(orderBy).ToArray()); }
            }
            else
            {
                return Json(await detailCall(key));
            }
        }
        #endregion

        protected static Task<IActionResult> AsTask(IActionResult result) => Task.FromResult(result);

        protected static ContentResult Json<TObject>(TObject obj) => AsJson(obj);
        protected static ContentResult AsJson<TContent>(TContent input)
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(input, typeof(TContent), SerializerSettings),
                ContentType = "application/json"
            };
        }
    }
}
