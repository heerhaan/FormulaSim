using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    public class TracksController : ViewDataController<Track>
    {
        public TracksController(FormulaContext context, PagingHelper pagingHelper) : base(context, pagingHelper)
        {
        }
        
        public IActionResult ArchivedTracks()
        {
            List<Track> tracks = Data.Where(t => t.Archived).OrderBy(t => t.Location).ToList();
            return View(tracks);
        }
    }
}
