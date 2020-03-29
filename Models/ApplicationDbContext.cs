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
        public DbSet<UserEvent> UserEvents { get; set; }

        public ApplicationDbContext(DbContextOptions options, IConfiguration config):base(options)
        {
            _config = config;

        }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("Default"));
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.UsersId, ue.EventsId });

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Users)
                .WithMany(u => u.Events)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(ue => ue.UsersId);
                

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Events)
                .WithMany(e => e.Volunteers)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(ue => ue.EventsId);
        }

        
    }
}
