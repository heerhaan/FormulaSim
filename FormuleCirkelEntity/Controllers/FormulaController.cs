using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Models;
using Newtonsoft.Json;

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
