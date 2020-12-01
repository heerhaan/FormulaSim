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
            // Finds the selected track by it's id
            Track track = await Data.FirstAsync(tr => tr.Id == id);
            // Finds the traits used by the given track and returns a list of it
            List<Trait> trackTraits = await _context.TrackTraits
                .Where(trt => trt.TrackId == id)
                .Select(trt => trt.Trait)
                .ToListAsync();

            // Finds the traits that belong to Tracks and aren't yet used by the given track
            List<Trait> traits = await _context.Traits
                .Where(tr => tr.TraitGroup == TraitGroup.Track && !trackTraits.Any(res => res.TraitId == tr.TraitId))
                .OrderBy(tr => tr.Name)
                .ToListAsync();

            if (track is null)
                return NotFound();

            var model = new TrackTraitsModel
            {
                Track = track,
                TrackTraits = trackTraits,
                Traits = traits
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrackTraits(int id, [Bind("TraitId")] int traitId)
        {
            Track track = await Data.FirstAsync(t => t.Id == id);
            Trait trait = await _context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (track is null || trait is null)
                return NotFound();

            TrackTrait newTrait = new TrackTrait { Track = track, Trait = trait };
            await _context.AddAsync(newTrait);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TrackTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{trackId}")]
        public async Task<IActionResult> RemoveTrait(int trackId, int traitId)
        {
            Track track = await Data.Include(tr => tr.TrackTraits).FirstAsync(t => t.Id == trackId);
            Trait trait = await _context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            TrackTrait removetrait = track.TrackTraits.First(trt => trt.TraitId == traitId);
            _context.Remove(removetrait);
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
