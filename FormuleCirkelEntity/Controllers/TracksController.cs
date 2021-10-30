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
        private readonly ITrackService _tracks;
        public TracksController(FormulaContext context, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper,
            ITrackService dataService)
            : base(context, userManager, pagingHelper, dataService)
        {
            _tracks = dataService;
        }

        [SortResult(nameof(Track.Location)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(base.Index().Result);
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int? id)
        {
            var updatingObject = await _tracks.GetTrackById(id.Value);
            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id, Track updatedObject)
        {
            updatedObject.Id = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (await _tracks.FirstOrDefault(res => res.Id == id) is null)
                return NotFound();

            _tracks.Update(updatedObject);
            await _tracks.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _tracks.GetTrackById(id.Value, true);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await _tracks.GetTrackById(id, true);
            if (objectToDelete == null)
                return NotFound();

            _tracks.Archive(objectToDelete);
            await _tracks.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("Traits/{id}")]
        public async Task<IActionResult> TrackTraits(int id)
        {
            // Finds the selected track by it's id
            Track track = await _tracks.GetTrackById(id);
            // Finds the traits used by the given track and returns a list of it
            List<Trait> trackTraits = await Context.TrackTraits
                .Where(trt => trt.TrackId == id)
                .Select(trt => trt.Trait)
                .ToListAsync();

            // Finds the traits that belong to Tracks and aren't yet used by the given track
            List<Trait> traits = Context.Traits
                .AsNoTracking()
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Track && !trackTraits.Any(res => res.TraitId == tr.TraitId))
                .OrderBy(tr => tr.Name)
                .ToList();

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
            Track track = await _tracks.GetTrackById(id);
            Trait trait = await Context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (track is null || trait is null)
                return NotFound();

            TrackTrait newTrait = new TrackTrait { Track = track, Trait = trait };
            DataService.Update(track);
            await Context.AddAsync(newTrait);
            await Context.SaveChangesAsync();

            return RedirectToAction(nameof(TrackTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{trackId}")]
        public async Task<IActionResult> RemoveTrait(int trackId, int traitId)
        {
            Track track = await Context.Tracks
                .Include(tr => tr.TrackTraits)
                .FirstAsync(t => t.Id == trackId);
            Trait trait = await Context.Traits
                .FirstAsync(tr => tr.TraitId == traitId);

            if (track == null || trait == null)
                return NotFound();

            TrackTrait removetrait = track.TrackTraits
                .First(trt => trt.TraitId == traitId);

            Context.Remove(removetrait);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(TrackTraits), new { id = trackId });
        }

        [Route("Archived")]
        public async Task<IActionResult> ArchivedTracks()
        {
            var tracks = await DataService.GetQueryable()
                .IgnoreQueryFilters()
                .Where(t => t.Archived)
                .OrderBy(t => t.Location)
                .ToListAsync();

            return View(tracks);
        }
    }
}
