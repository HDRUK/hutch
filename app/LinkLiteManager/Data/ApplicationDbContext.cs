using LinkLiteManager.Data.Entities;
using LinkLiteManager.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkLiteManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }

  public DbSet<RegistrationAllowlistEntry> RegistrationAllowlist => Set<RegistrationAllowlistEntry>();

  public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

  public DbSet<ActivitySource> ActivitySources => Set<ActivitySource>();

  public DbSet<SourceType> SourceTypes => Set<SourceType>();

  public DbSet<Logs> Logs => Set<Logs>();

  public DbSet<Person> Person => Set<Person>();

  public DbSet<ConditionOccurrence> ConditionOccurrence => Set<ConditionOccurrence>();
  public DbSet<Measurement> Measurement => Set<Measurement>();
  public DbSet<Observation> Observation => Set<Observation>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Person>()
      .HasMany(p => p.ConditionOccurrences)
      .WithOne(co => co.Person)
      .HasForeignKey("PersonId");
    modelBuilder.Entity<Person>()
      .HasMany(p => p.Observations)
      .WithOne(o => o.Person)
      .HasForeignKey("PersonId");
    modelBuilder.Entity<Person>()
      .HasMany(p => p.Measurements)
      .WithOne(m => m.Person)
      .HasForeignKey("PersonId");

  }

}
