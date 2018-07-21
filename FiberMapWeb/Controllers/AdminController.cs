using System;
using System.Collections.Generic;
using System.Linq;
using FiberMapWeb.Database;
using FiberMapWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FiberMapWeb.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            List<FiberLocation> locations = new List<FiberLocation>();

            using (var db = new FiberMapWeb.Database.FiberMapContext())
            {
                foreach (var location in db.FiberLocations)
                {
                    locations.Add(location);
                }
            }

            ListViewModel model = new ListViewModel();
            model.Locations = locations;
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id) {
            using (var db = new FiberMapWeb.Database.FiberMapContext())
            {
                var record = db.FiberLocations.Where(x => x.LocationId == id).FirstOrDefault();

                if(record != null)
                {
                    try
                    {
                        db.FiberLocations.Remove(record);
                        db.SaveChanges();
                    } catch(Exception ex) {
                        ModelState.AddModelError("","Unable to Delete Record");
                        System.Diagnostics.Debug.WriteLine("Error Removing Record: {0}", ex.Message);
                    }
                }
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (!id.HasValue) {
                // New Entry
                return View(new FiberLocation(){LocationId = 0, Latitude = 0, Longitude = 0});
            } else {
                // lookup entry
                using (var db = new FiberMapContext()) {
                    var findRec = db.Find(typeof(FiberLocation), new object[] { id.Value });
                    if (findRec != null) {
                        FiberLocation editRec = (FiberMapWeb.Database.FiberLocation)findRec;
                        return View(editRec);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult Detail(FiberLocation location) {
            if (location == null) {
                ModelState.AddModelError("", "Location Was Null");
            } else {
                if(ModelState.IsValid) {
                    if(string.IsNullOrWhiteSpace(location.Address)) {
                        ModelState.AddModelError("", "Please Enter Address!");
                    }

                    if (string.IsNullOrWhiteSpace(location.City)) {
                        ModelState.AddModelError("", "Please Enter City!");
                    }

                    if (string.IsNullOrWhiteSpace(location.State)) {
                        ModelState.AddModelError("", "Please Enter State!");
                    }

                    if (string.IsNullOrWhiteSpace(location.Zip)) {
                        ModelState.AddModelError("", "Please Enter Zip!");
                    }

                    if(Math.Abs(location.Latitude) < 0) {
                        ModelState.AddModelError("", "Latitude Not Entered");
                    }

                    if(Math.Abs(location.Longitude) < 0) {
                        ModelState.AddModelError("", "Longitude Not Entered");
                    }

                    using (var db = new FiberMapWeb.Database.FiberMapContext())
                    {
                        if (location.LocationId == 0 && db.FiberLocations.Where(x => x.Address == location.Address && x.City == location.City && x.State == location.State).Any())
                        {
                            ModelState.AddModelError("", "Address Has Already Been Added");
                        }
                    }

                    if(ModelState.IsValid) {
                        SaveLocation(location);
                        Response.Redirect(Url.Action("List","Admin"));
                    }
                }                
            }

            return View(location);
        }

        private void SaveLocation(FiberLocation location) {
            using (var db = new FiberMapWeb.Database.FiberMapContext())
            {
                if (location.LocationId == 0)
                {
                    db.Add(location);
                }
                else
                {
                    db.Update(location);
                }

                db.SaveChanges();
            }
        }
    }
}
