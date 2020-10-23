using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Filters
{
    public class PagedResultAttribute : ActionFilterAttribute
    {
        const string PAGE_KEY = "p";
        const string PAGE_SIZE_KEY = "ps";
        int _defaultPageSize;
        MethodInfo _skipMethod;
        MethodInfo _takeMethod;

        public PagedResultAttribute()
            : this(15)
        { }

        public PagedResultAttribute(int defaultPageSize)
        {
            _skipMethod = typeof(Queryable).GetMethod(nameof(Queryable.Skip));
            _takeMethod = typeof(Queryable).GetMethod(nameof(Queryable.Take));
            _defaultPageSize = defaultPageSize;
            Order = 0;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            if (executedContext.Result is ViewResult result && result.Model is IQueryable<ModelBase> model)
            {
                var itemCount = await model.CountAsync();
                var requestData = GetRequestPagingData(executedContext.HttpContext.Request);
                var pageSize = PagingHelper.CheckPageSize(requestData.pageSize);
                var pageNumber = requestData.page > 0 ? requestData.page : 1;
                var pageCount = PagingHelper.GetPageCount(pageSize, itemCount);

                result.ViewData.Model = SetPageQuery(model, pageNumber, pageSize);
                result.ViewData.Add("pageCount", pageCount);
                result.ViewData.Add("pageSize", pageSize);
                result.ViewData.Add("pageNumber", pageNumber);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        object SetPageQuery(IQueryable queryable, int page, int pageSize)
        {
            try
            {
                var skip = _skipMethod.MakeGenericMethod(queryable.ElementType)
                    .Invoke(null, new object[] { queryable, pageSize * (page - 1) });
                return _takeMethod.MakeGenericMethod(queryable.ElementType)
                    .Invoke(null, new object[] { skip, pageSize });
            }
            catch
            {
                return queryable;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        (int page, int pageSize) GetRequestPagingData(HttpRequest request)
        {
            var pageSize = int.Parse(GetQueryValue(request, PAGE_SIZE_KEY, _defaultPageSize.ToString()));
            var page = int.Parse(GetQueryValue(request, PAGE_KEY, "1"));
            return (page, pageSize);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
        static string GetQueryValue(HttpRequest request, string queryKey, string defaultValue = null)
        {
            KeyValuePair<string, StringValues> sortKey = request.Query
                .LastOrDefault(q => q.Key.ToLowerInvariant() == queryKey);
            string value = sortKey.Value.FirstOrDefault();
            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }
    }
}
