using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<T> FindAsync<T>(this IQueryable<T> collection, int id)
            where T : ModelBase
        {
            return await collection.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
