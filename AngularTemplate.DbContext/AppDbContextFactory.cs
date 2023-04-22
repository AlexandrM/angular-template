namespace AngularTemplate.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public static AppDbContextFactory Instance = new();

	public AppDbContext CreateDbContext()
	{
		return CreateDbContext(null);
	}

	public AppDbContext CreateDbContext(string[]? args)
	{
		var connection = RepositoryInitializer.ConnectionString;

		var builder = new DbContextOptionsBuilder<AppDbContext>();

		var provider = connection?.Provider ?? args?[0] ?? "";
		var connectionString = connection?.ConnectionString ?? args?[1] ?? "";

		if (!Enum.TryParse<AppDbContextProvidersEnum>(provider, out var providerEnum))
			throw new InvalidOperationException($"No such provider: [{provider}]");

		switch (providerEnum)
		{
			case AppDbContextProvidersEnum.npgsql:
				builder.UseNpgsql(connectionString, x => x.MigrationsAssembly("AngularTemplate.Migrations.Postgres"));
				break;
			case AppDbContextProvidersEnum.sqlite:
				builder.UseSqlite(connectionString, x => x.MigrationsAssembly("AngularTemplate.Migrations.Sqlite"));
				break;

			default:
				throw new NotSupportedException($"No such provider: [{provider}]");
		}

		var db = new AppDbContext(providerEnum, builder.Options);
		db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

		return db;
	}
}