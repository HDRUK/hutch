using HutchManager.Data.Entities;
using HutchManager.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HutchManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options) { }

  public DbSet<RegistrationAllowlistEntry> RegistrationAllowlist => Set<RegistrationAllowlistEntry>();

  public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

  public DbSet<ActivitySource> ActivitySources => Set<ActivitySource>();

  public DbSet<DataSource> DataSources => Set<DataSource>();

  public DbSet<SourceType> SourceTypes => Set<SourceType>();

  public DbSet<ModifierType> ModifierTypes => Set<ModifierType>();

  public DbSet<ResultsModifier> ResultsModifiers => Set<ResultsModifier>();
  
  public DbSet<Agent> Agents => Set<Agent>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }

}
