
using IWeddySupport.Model;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace IWeddySupport.Repository
{
    public class IWeddySupportDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public IWeddySupportDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IWeddySupportDbContext(DbContextOptions<IWeddySupportDbContext> options, IConfiguration configuration)
       : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<EducationalQualification> EducationalQualifications { get; set; }
        public DbSet<ProfilePhoto> ProfilePhotos { get; set; }
        public DbSet<PartnerExpectation> PartnerExpectations { get; set; }
        public DbSet<UserRequest> UserRequests { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)),
                    options => options
                        .EnableRetryOnFailure(
                            maxRetryCount: 5, // Maximum number of retry attempts
                            maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                            errorNumbersToAdd: null // Optionally specify error numbers to consider for retry
                        ));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.OwnsOne(a => a.PermanentAddress);
                entity.OwnsOne(a => a.CurrentAddress);
            });
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}
