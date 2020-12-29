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
    public interface IChampionshipService
    {
        IQueryable<Championship> GetChampionshipsQuery();
        Task<IList<Championship>> GetChampionships();
        Task<Championship> GetChampionshipById(int id);
        Task<Championship> FirstOrDefault(Expression<Func<Championship, bool>> predicate);
        Task Add(Championship championship);
        void Update(Championship championship);
        Task ActivateChampionship(Championship championship);
        Task SaveChangesAsync();
    }

    public class ChampionshipService : IChampionshipService
    {
        private readonly FormulaContext _context;
        private DbSet<Championship> Data { get; }

        public ChampionshipService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<Championship>();
        }

        public IQueryable<Championship> GetChampionshipsQuery()
        {
            return Data;
        }

        public async Task<IList<Championship>> GetChampionships()
        {
            var championship = await Data.AsNoTracking().ToListAsync();
            return championship;
        }

        public async Task<Championship> GetChampionshipById(int id)
        {
            var item = await Data.AsNoTracking()
                .FirstOrDefaultAsync(res => res.ChampionshipId == id);
            return item;
        }

        public async Task<Championship> FirstOrDefault(Expression<Func<Championship, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task Add(Championship championship)
        {
            await Data.AddAsync(championship);
        }

        public void Update(Championship championship)
        {
            Data.Update(championship);
        }

        public async Task ActivateChampionship(Championship championship)
        {
            if (championship is null) { throw new NullReferenceException(); }
            // First find and de-activate the other championships
            var otherChamps = await Data.Where(c => c.ActiveChampionship).ToListAsync();
            foreach (var champ in otherChamps)
                champ.ActiveChampionship = false;

            Data.UpdateRange(otherChamps);
            // Then activate the given championship
            championship.ActiveChampionship = true;
            Update(championship);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
