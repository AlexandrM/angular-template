namespace AngularTemplate.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using DbContext;

public class RepositoryBase<TEntity> where TEntity : class
{
	protected readonly AppDbContext _context;

	protected RepositoryBase(
		AppDbContext context
	)
	{
		_context = context;
	}

	protected async Task<int> SaveChangesAsync()
	{
		try
		{
			return await _context.SaveChangesAsync();
		}
		catch (DbUpdateException updateException)
		{
			throw updateException;
		}
	}

	protected int SaveChanges()
	{
		try
		{
			return _context.SaveChanges();
		}
		catch (DbUpdateException updateException)
		{
			throw updateException;
		}
	}


	protected async Task<TEntity?> GetBaseAsync(Guid id)
	{
		return await _context.Set<TEntity>().FindAsync(id);
	}

	protected async Task<int> AddBaseAsync(TEntity entity)
	{
		await _context.Set<TEntity>().AddAsync(entity);
		return await SaveChangesAsync();
	}

	protected int AddBase(TEntity entity)
	{
		_context.Set<TEntity>().Add(entity);
		return SaveChanges();
	}

	protected int RemoveBase(TEntity entity)
	{
		_context.Set<TEntity>().Remove(entity);
		return SaveChanges();
	}

	protected async Task<int> UpdateBaseAsync(TEntity entity)
	{
		_context.Attach(entity);
		_context.Entry(entity).State = EntityState.Modified;

		return await SaveChangesAsync();
	}

	protected int UpdateBase(TEntity entity)
	{
		_context.Attach(entity);
		return SaveChanges();
	}
}