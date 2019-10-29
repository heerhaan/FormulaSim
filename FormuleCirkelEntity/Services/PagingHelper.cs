using System;
using System.Linq;

namespace FormuleCirkelEntity.Services
{
    public class PagingHelper
    {
        const int MAX_PAGE_SIZE = 100;
        const int DEFAULT_PAGE_SIZE = 10;

        public int CheckPageSize(int pageSize)
        {
            if(pageSize <= 0)
                pageSize = DEFAULT_PAGE_SIZE;
            if(pageSize > MAX_PAGE_SIZE)
                pageSize = MAX_PAGE_SIZE;
            return pageSize;
        }

        public int GetPageCount(int pageSize, int itemCount)
        {
            var pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemCount / pageSize)));
            if (pageCount <= 0)
                pageCount = 1;
            return pageCount;
        }
    }
}
