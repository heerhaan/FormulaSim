using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class TracksController : ViewDataController<Track>
    {
        public TracksController(FormulaContext context, PagingHelper pagingHelper) : base(context, pagingHelper)
        {
        }

        [SortResult(nameof(Track.Location)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return base.Index();
        }
        
        [Route("Traits/{id}")]
        public async Task<IActionResult> TrackTraits(int id)
        {
            var track = await Data.IgnoreQueryFilters()
                .SingleOrDefaultAsync(t => t.Id == id);

            var traits = DataContext.Traits
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Track && !track.Traits.Values.Contains(tr))
                .OrderBy(t => t.Name)
                .ToList();

            if (track == null)
                return NotFound();

            var model = new TraitsTrackModel
            {
                Track = track,
                Traits = traits
            };

            return View(model);
        }

        [HttpPost("Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrackTraits(int id, [Bind("TraitId")] int traitId)
        {
            var track = await Data.SingleOrDefaultAsync(t => t.Id == id);
            var trait = await DataContext.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            track.Traits.Add(track.Traits.Count, trait);
            DataContext.Update(track);
            await DataContext.SaveChangesAsync();

            return RedirectToAction(nameof(TrackTraits), new { id });
        }

        [Route("Traits/Remove/{trackId}")]
        public async Task<IActionResult> RemoveTrait(int trackId, int traitId)
        {
            var track = await Data.SingleOrDefaultAsync(t => t.Id == trackId);
            var trait = await DataContext.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            var removetrait = track.Traits.First(item => item.Value.TraitId == trait.TraitId);
            track.Traits.Remove(removetrait);
            DataContext.Update(track);
            await DataContext.SaveChangesAsync();

            return RedirectToAction(nameof(TrackTraits), new { id = trackId });
        }

        [Route("Archived")]
        public IActionResult ArchivedTracks()
        {
            List<Track> tracks = Data.IgnoreQueryFilters()
                .Where(t => t.Archived)
                .OrderBy(t => t.Location)
                .ToList();

            return View(tracks);
        }
    }
}
