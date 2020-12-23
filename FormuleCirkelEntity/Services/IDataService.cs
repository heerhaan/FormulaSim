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
    public interface IDataService<T> where T : ModelBase
    {
        IQueryable<T> GetQueryable();
        Task<IList<T>> GetEntities();
        Task<IList<T>> GetEntitiesUnfiltered();
        Task<T> GetEntityById(int id);
        Task<T> GetEntityByIdUnfiltered(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        void Update(T entity);
        void Archive(T entity);
        Task SaveChangesAsync();
    }

    public class DataService<T> : IDataService<T> where T : ModelBase
    {
        private readonly FormulaContext _context;
        private DbSet<T> Data { get; }

        public DataService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return Data;
        }

        public async Task<IList<T>> GetEntities()
        {
            var items = await Data.AsNoTracking().ToListAsync();
            return items;
        }

        public async Task<IList<T>> GetEntitiesUnfiltered()
        {
            var items = await Data.IgnoreQueryFilters().AsNoTracking().ToListAsync();
            return items;
        }

        public async Task<T> GetEntityById(int id)
        {
            var item = await Data.AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
            return item;
        }

        public async Task<T> GetEntityByIdUnfiltered(int id)
        {
            return await Data.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task Add(T entity)
        {
            await Data.AddAsync(entity);
        }

        public void Update(T entity)
        {
            Data.Update(entity);
        }

        public void Archive(T entity)
        {
            // First check if entity is archived or not, based on that either archive or unarchive the entity
            if (entity is IArchivable archivable && archivable.Archived)
            {
                _context.Restore(archivable);
                Update(entity);
            }
            else
            {
                _context.Remove(entity);
            } 
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
