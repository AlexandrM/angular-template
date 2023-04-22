namespace AngularTemplate.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DbContext;
using Shared;

public class AppDbContextFactoryPostgres : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[]? args)
	{
		var builder = new DbContextOptionsBuilder<AppDbContext>();

		var sharedConfiguration = SharedConfiguration.CreateConfigurationContainer();
		var connectionString =
			args?.FirstOrDefault()
			?? sharedConfiguration.GetConnectionString("npgsql");

		builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("AngularTemplate.Migrations.Postgres"));

		var db = new AppDbContext(AppDbContextProvidersEnum.npgsql, builder.Options);
		db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

		return db;
	}
}