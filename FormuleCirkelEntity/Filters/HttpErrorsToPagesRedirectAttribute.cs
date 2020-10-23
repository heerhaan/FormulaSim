using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace FormuleCirkelEntity.Filters
{
    public class HttpErrorsToPagesRedirectAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (context is null)
                return;
            if(context.Result is NotFoundResult)
                context.Result = new RedirectResult("/NotFound");
        }
    }
}
