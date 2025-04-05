using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DanceStudioFinder.DataBase
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }/*
        public DbSet<Studio> studios { get; set; }
        public DbSet<Address> addresses { get; set; }*/

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
