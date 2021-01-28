using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface IChampionshipService : IDataService<Championship>
    {
        Task<IList<Championship>> GetChampionships();
        Task<Championship> GetChampionshipById(int id, bool includeRanges = false);
        Task ActivateChampionship(Championship championship);
        string SetRangeToChampionship(Championship championship, bool isAgeDev, IList<int> valueKeys, IList<int> valueMins, IList<int> valueMaxs);
    }

    public class ChampionshipService : DataService<Championship>, IChampionshipService
    {
        public ChampionshipService(FormulaContext context) : base(context) { }

        public async Task<IList<Championship>> GetChampionships()
        {
            return await Data.AsNoTracking().ToListAsync();
        }

        public async Task<Championship> GetChampionshipById(int id, bool includeRanges = false)
        {
            return await Data.AsNoTracking()
                .If(includeRanges, res => res.Include(c => c.AgeDevRanges))
                .If(includeRanges, res => res.Include(c => c.SkillDevRanges))
                .FirstOrDefaultAsync(res => res.ChampionshipId == id);
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

        public string SetRangeToChampionship(Championship championship, bool isAgeDev,
            IList<int> valueKeys,
            IList<int> valueMins,
            IList<int> valueMaxs)
        {
            // Null check to ensure all given parameters actually contain a value
            if (championship is null || valueKeys is null || valueMins is null || valueMaxs is null)
                return "Values couldn't be read";

            string errString = "";
            // This ensures the lists are of equal length, if this isn't the case then it returns a string
            errString += MinMaxDevValidator.ValidateListsEqualLength(valueKeys.Count, valueMins.Count, valueMaxs.Count);
            // We have to ensure that the order of the keys are correct, otherwise returns a bad
            errString += MinMaxDevValidator.CheckIfListIsInOrder(valueKeys);
            // If the lists are neither in order nor have an equal length then we return it
            if (!string.IsNullOrEmpty(errString))
                return errString;
            // Empties either of the dev ranges so we can put in some new ones (owo)
            if (isAgeDev)
                Context.MinMaxDevRange.RemoveRange(championship.AgeDevRanges);
            else
                Context.MinMaxDevRange.RemoveRange(championship.SkillDevRanges);

            // Loops through all the key values which assumes that the age value, min dev and max dev lists are of the same length
            for (int i = 0; i < valueKeys.Count; i++)
            {
                // Creates an MinMax object from the given lists, assuming they are made equally
                MinMaxDevRange newDevRange = new MinMaxDevRange
                {
                    ValueKey = valueKeys[i],
                    MinDev = valueMins[i],
                    MaxDev = valueMaxs[i]
                };

                // Validates the current entry
                var validate = MinMaxDevValidator.ValidateMinMax(newDevRange);
                if (!validate.IsValid)
                {
                    foreach (var failure in validate.Errors)
                        errString += $"{failure.ErrorMessage} ";
                }
                else
                {
                    // Validator didn't trigger so this line can be added to the ranges
                    if (isAgeDev)
                        championship.AgeDevRanges.Add(newDevRange);
                    else
                        championship.SkillDevRanges.Add(newDevRange);
                }
            }
            return errString;
        }
    }
}
