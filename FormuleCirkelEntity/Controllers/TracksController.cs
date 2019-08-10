using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class TracksController : ViewDataController<Track>
    {
        public TracksController(FormulaContext context, PagingHelper pagingHelper) : base(context, pagingHelper)
        {
        }
        
        [Route("Archived")]
        public IActionResult ArchivedTracks()
        {
            List<Track> tracks = Data.IgnoreQueryFilters().Where(t => t.Archived).OrderBy(t => t.Location).ToList();
            return View(tracks);
        }
    }
}
