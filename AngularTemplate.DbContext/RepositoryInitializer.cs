namespace AngularTemplate.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public record ConnectionConfiguration(string Provider, string ConnectionString);

public class RepositoryInitializer
{
	public static void Initialize(
		IServiceCollection services,
		ConnectionConfiguration connectionString
	)
	{
		Services = services!;
		ConnectionString = connectionString;

		var db = AppDbContextFactory.Instance.CreateDbContext();
		db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		//db.Database.EnsureCreated();
		db.Database.Migrate();
	}

	public static ConnectionConfiguration ConnectionString { get; internal set; } = null!;
	public static IServiceCollection Services { get; internal set; } = null!;
}