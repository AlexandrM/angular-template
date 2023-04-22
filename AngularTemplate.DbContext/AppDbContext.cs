namespace AngularTemplate.DbContext;

using Microsoft.EntityFrameworkCore;
using Data.Models.Users;

public enum AppDbContextProvidersEnum
{
	sqlite,
	mssql,
	npgsql
}

public class AppDbContext : DbContext
{
	private readonly AppDbContextProvidersEnum _provider;

	private readonly Dictionary<AppDbContextProvidersEnum, string> _noCaseCollation = new()
	{
		{ AppDbContextProvidersEnum.sqlite, "NOCASE" },
		{ AppDbContextProvidersEnum.npgsql, "my_ci_collation" }
	};

	public AppDbContext(AppDbContextProvidersEnum provider, DbContextOptions<AppDbContext> options) : base(options)
	{
		_provider = provider;
	}

	public DbSet<User> Users { get; set; } = null!;
	public DbSet<UserExternal> UserClaims { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var noCaseCollation = _noCaseCollation[_provider];

		if (_provider == AppDbContextProvidersEnum.npgsql)
			modelBuilder.HasCollation("my_ci_collation", "en-u-ks-primary", "icu", false);

		/* NOCASE PROPERTIES */
		modelBuilder.Entity<User>()
			.Property(x => x.UserName)
			.UseCollation(noCaseCollation);

		modelBuilder.Entity<User>()
			.Property(x => x.Email)
			.UseCollation(noCaseCollation);

		modelBuilder.Entity<UserExternal>()
			.Property(x => x.Provider)
			.UseCollation(noCaseCollation);

		modelBuilder.Entity<UserExternal>()
			.Property(x => x.Email)
			.UseCollation(noCaseCollation);


		modelBuilder.Entity<UserExternal>()
			.HasOne(x => x.User)
			.WithMany(x => x.UserClaims)
			.HasForeignKey(x => x.UserId);

		base.OnModelCreating(modelBuilder);
	}
}