using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleAPI.Models;

namespace ExampleAPI.Models
{
    public class ApplicationDbContext: DbContext
    {
        IConfiguration _config;
        public DbSet<UserModel> Users { get; set; }
        public DbSet<EventModel> Events { get; set; }

        public ApplicationDbContext(DbContextOptions options, IConfiguration config):base(options)
        {
            _config = config;

        }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_config.GetConnectionString("Default"));
        }
        

        public DbSet<ExampleAPI.Models.EventModel> EventModel { get; set; }
    }
}
