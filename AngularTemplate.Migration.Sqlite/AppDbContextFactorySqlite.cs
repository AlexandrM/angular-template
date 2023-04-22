namespace AngularTemplate.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DbContext;
using Shared;

public class AppDbContextFactorySqlite : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[]? args)
	{
		var builder = new DbContextOptionsBuilder<AppDbContext>();

		var sharedConfiguration = SharedConfiguration.CreateConfigurationContainer();
		var connectionString =
			args?.FirstOrDefault()
			?? sharedConfiguration.GetConnectionString("sqlite");

		builder.UseSqlite(connectionString, b => b.MigrationsAssembly("AngularTemplate.Migrations.Sqlite"));

		var db = new AppDbContext(AppDbContextProvidersEnum.sqlite, builder.Options);
		db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

		return db;
	}
}