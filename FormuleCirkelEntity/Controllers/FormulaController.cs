using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class FormulaController : Controller
    {
        protected static Task<IActionResult> AsTask(IActionResult result)
        {
            return Task.FromResult(result);
        }
    }
}
