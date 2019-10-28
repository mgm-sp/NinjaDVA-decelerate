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
    }
}
