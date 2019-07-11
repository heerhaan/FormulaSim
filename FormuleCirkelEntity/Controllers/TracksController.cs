using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace FormuleCirkelEntity.Controllers
{
    public class TracksController : Controller
    {
        const string VIEW_CREATEORUPDATE = "Modify";

        private readonly FormulaContext _context;

        public TracksController(FormulaContext context)
        {
            _context = context;
        }

        // GET: Tracks
        public IActionResult Index(int? page)
        {
            var tracks = _context.Tracks.Where(t => t.Archived == false);
            var pageNumber = page ?? 1;
            var onePageOfTracks = tracks.ToPagedList(pageNumber, 10);
            ViewBag.OnePage = onePageOfTracks;
            return View();
        }
        
        // GET: Tracks/Create
        public IActionResult Create()
        {
            var track = new Track();
            return View(VIEW_CREATEORUPDATE, track);
        }

        // POST: Tracks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Track track)
        {
            if (ModelState.IsValid)
            {
                _context.Add(track);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(VIEW_CREATEORUPDATE, track);
        }

        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track == null)
                return NotFound();

            return View(VIEW_CREATEORUPDATE, track);
        }

        // POST: Tracks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Track modifiedTrack)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                track.Name = modifiedTrack.Name;
                track.Location = modifiedTrack.Location;
                track.DNFodds = modifiedTrack.DNFodds;
                track.RNGodds = modifiedTrack.RNGodds;
                track.Specification = modifiedTrack.Specification;
                track.LengthKM = modifiedTrack.LengthKM;

                _context.Update(track);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(VIEW_CREATEORUPDATE, modifiedTrack);
        }

        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .FirstOrDefaultAsync(m => m.TrackId == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track.Archived == false)
            {
                track.Archived = true;
                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();
            }
            else
            {
                track.Archived = false;
                _context.Tracks.Update(track);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ArchivedTracks()
        {
            var tracks = _context.Tracks.Where(t => t.Archived).ToList();
            return View(tracks);
        }
    }
}
