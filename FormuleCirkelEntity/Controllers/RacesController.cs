using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormuleCirkelEntity.Controllers
{
    public class RacesController : Controller
    {
        private readonly FormulaContext _context;

        public RacesController(FormulaContext context)
        {
            _context = context;
        }
        public IActionResult AddTracks()
        {
            return View(_context.Tracks.ToList());
        }

        /*public async Task<IActionResult> AddTracks(int seasonid)
        {
            return View();
        }*/
    }
}