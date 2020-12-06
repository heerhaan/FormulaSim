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
        IQueryable<T> GetEntities();
        Task<IEnumerable<T>> GetAllEntities();
        Task<T> GetEntity(int id);
        Task<T> GetAnyEntity(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        void Update(T entity);
        void Archive(T entity);
        Task SaveChangesAsync();
    }

    public class DataService<T> : IDataService<T> where T : ModelBase
    {
        private FormulaContext _context;
        private DbSet<T> Data { get; }

        public DataService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<T>();
        }

        public IQueryable<T> GetEntities()
        {
            return Data;
        }

        public async Task<IEnumerable<T>> GetAllEntities()
        {
            return await Data.IgnoreQueryFilters().ToListAsync();
        }

        public async Task<T> GetEntity(int id)
        {
            return await Data.FindAsync(id);
        }

        public async Task<T> GetAnyEntity(int id)
        {
            return await Data.IgnoreQueryFilters().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return await Data.FirstOrDefaultAsync(predicate);
        }

        public async Task Add(T entity)
        {
            await Data.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Archive(T entity)
        {
            if (entity is IArchivable archivable && archivable.Archived)
                _context.Restore(archivable);
            else
                _context.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
