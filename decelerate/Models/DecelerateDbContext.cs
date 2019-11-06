using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using decelerate.Models;
using Microsoft.EntityFrameworkCore;

namespace decelerate.Models
{
    public class DecelerateDbContext : DbContext
    {
        public DecelerateDbContext(DbContextOptions<DecelerateDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Presenter> Presenters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Create composite key for the user model: */
            modelBuilder.Entity<User>().HasKey(u => new { u.Name, u.RoomId });
        }
    }
}
