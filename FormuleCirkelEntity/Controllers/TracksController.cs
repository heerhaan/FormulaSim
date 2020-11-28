using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        public TracksController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper)
            : base(context, identityContext, userManager, pagingHelper)
        {
        }

        [SortResult(nameof(Track.Location)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            return base.Index().Result;
        }
        
        [Route("Traits/{id}")]
        public async Task<IActionResult> TrackTraits(int id)
        {
            var track = await Data.IgnoreQueryFilters()
                .SingleOrDefaultAsync(t => t.Id == id);

            var traits = _context.Traits
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Track && !track.Traits.Any(res => res.TraitId == tr.TraitId))
                .OrderBy(t => t.Name)
                .ToList();

            if (track == null)
                return NotFound();

            var model = new TrackTraitsTrackModel
            {
                Track = track,
                Traits = traits
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrackTraits(int id, [Bind("TraitId")] int traitId)
        {
            var track = await Data.SingleOrDefaultAsync(t => t.Id == id);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            track.Traits.Add(trait);
            _context.Update(track);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TrackTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{trackId}")]
        public async Task<IActionResult> RemoveTrait(int trackId, int traitId)
        {
            var track = await Data.SingleOrDefaultAsync(t => t.Id == trackId);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            var removetrait = track.Traits.First(item => item.TraitId == trait.TraitId);
            track.Traits.Remove(removetrait);
            _context.Update(track);
            await _context.SaveChangesAsync();

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
