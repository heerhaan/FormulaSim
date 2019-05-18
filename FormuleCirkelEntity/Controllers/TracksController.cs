using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tracks.ToListAsync());
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

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
