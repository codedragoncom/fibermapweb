using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FiberMapWeb.Models;
using FiberMapWeb.Database;
using Microsoft.AspNetCore.Authorization;

namespace FiberMapWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetLocations()
        {
            List<FiberLocation> locations = new List<FiberLocation>();

            using (var db = new FiberMapWeb.Database.FiberMapContext())
            {
                foreach (var location in db.FiberLocations)
                {
                    locations.Add(location);
                }
            }

            return Json(locations.ToList());
        }

        public IActionResult ViewMap()
        {
            ViewData["GoogleAPIKey"] = Startup.googleAPIKey;
            return View();
        }
    }
}
