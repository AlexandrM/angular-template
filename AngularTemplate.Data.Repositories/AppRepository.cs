namespace AngularTemplate.Data.Repositories;

using DbContext;

public class AppRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
{
	public AppRepository() : base(AppDbContextFactory.Instance.CreateDbContext())
	{
	}
}