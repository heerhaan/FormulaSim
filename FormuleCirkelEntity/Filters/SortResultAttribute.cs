using FormuleCirkelEntity.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FormuleCirkelEntity.Filters
{
    public class SortResultAttribute : ActionFilterAttribute
    {
        const string SORT_KEY = "s";

        MethodInfo _sortMethod;
        string _defaultSortProperty;

        public SortResultAttribute() : this(null)
        { }

        public SortResultAttribute(string defaultSortProperty)
        {
            _sortMethod = typeof(IQueryableExtensions).GetMethod(nameof(IQueryableExtensions.OrderBy));
            _defaultSortProperty = defaultSortProperty;
            Order = 10;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if (context is null)
                return;
            if(context.Result is ViewResult result && result.Model is IQueryable queryable)
            {
                string sortName = GetSortProperty(context.HttpContext.Request);

                if(sortName == null)
                    return;

                try
                {
                    result.ViewData.Model = _sortMethod
                        .MakeGenericMethod(queryable.ElementType)
                        .Invoke(null, new[] { result.Model, sortName });
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
        string GetSortProperty(HttpRequest request)
        {
            KeyValuePair<string, StringValues> sortKey = request.Query
                .LastOrDefault(q => q.Key.ToLowerInvariant() == SORT_KEY);
            if(sortKey.Equals(default(KeyValuePair<string, StringValues>)))
                return _defaultSortProperty;
            string value = sortKey.Value.FirstOrDefault();
            return !string.IsNullOrWhiteSpace(value) ? value : _defaultSortProperty;
        }
    }
}
