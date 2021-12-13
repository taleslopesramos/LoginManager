using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginManager.Model.Context
{
    public class LoginManagerDbContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public LoginManagerDbContext(){}
        public LoginManagerDbContext(DbContextOptions<LoginManagerDbContext> options): base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user =>
            {
                user.HasMany(u => u.Profiles)
                .WithOne(profile => profile.User)
                .HasForeignKey(p => p.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
