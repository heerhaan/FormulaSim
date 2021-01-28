using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface IDataService<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        void Update(T entity);
        void Archive(T entity);
        Task SaveChangesAsync();
    }

    public class DataService<T> : IDataService<T> where T : class
    {
        protected FormulaContext Context { get; }
        protected DbSet<T> Data { get; }

        public DataService(FormulaContext context)
        {
            Context = context;
            Data = Context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return Data;
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task Add(T entity)
        {
            await Context.AddAsync(entity);
        }

        public void Update(T entity)
        {
            Context.Update(entity);
        }

        public void Archive(T entity)
        {
            // First check if entity is archived or not, based on that either archive or unarchive the entity
            if (entity is IArchivable archivable && archivable.Archived)
            {
                Context.Restore(archivable);
                Update(entity);
            }
            else
            {
                Context.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
