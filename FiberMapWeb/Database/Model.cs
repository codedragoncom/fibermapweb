using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FiberMapWeb.Database
{
    public class FiberMapContext : DbContext
    {
        public DbSet<FiberLocation> FiberLocations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string pathToDb = string.Format("fiber_locations.db");
            optionsBuilder.UseSqlite(string.Format("Data Source={0}", pathToDb));
        }
    }

    public class FiberLocation
    {
        [Key]
        [HiddenInput]
        public int LocationId { get; set; }
        public string Address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Service_type { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
