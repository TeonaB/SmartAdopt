using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Models;

namespace SmartAdopt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<RaspAnimal> RaspAnimals { get; set; }
        public DbSet<RaspChestionar> RaspChestionars { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Postare> Postares { get; set; }
        public DbSet<Comentariu> Comentarius { get; set; }
        public DbSet<Comanda> Comandas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comanda>()
                .HasOne(c => c.Animal)
                .WithMany() 
                .HasForeignKey(c => c.idAnimal);
            
            modelBuilder.Entity<Comanda>()
                .HasOne(c => c.Client)
                .WithMany() 
                .HasForeignKey(c => c.idClient);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId);

            base.OnModelCreating(modelBuilder);

        }
    }
}
