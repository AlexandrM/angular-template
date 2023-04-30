namespace AngularTemplate.Data.Repositories.Users;

using AngularTemplate.Data.Models.Users;
using Repositories;
using Microsoft.EntityFrameworkCore;

public class UserExternalRepository : AppRepository<UserExternal>, IUserExternalRepository
{
	public async Task<int> AddUserExternalAsync(UserExternal userExternal)
	{
		return await AddBaseAsync(userExternal);
	}

	public int AddUserExternal(UserExternal userExternal)
	{
		return AddBase(userExternal);
	}

	public async Task<int> AddUserExternalsAsync(IEnumerable<UserExternal> claims)
	{
		_context.AddRange(claims);
		return await SaveChangesAsync();
	}


	public async Task<List<UserExternal>> GetUserExternalsByUserNameAsync(string userName)
	{
		return await _context
			.UserClaims
			.Where(x => x.User.UserName == userName)
			.Include(x => x.User)
			.ToListAsync();
	}

	public async Task<List<UserExternal>> GetUserExternalsByUserIdAsync(Guid userId)
	{
		return await _context
			.UserClaims
			.Where(x => x.UserId == userId)
			.Include(x => x.User)
			.ToListAsync();
	}

	public List<UserExternal> GetUserExternalsByUser(Guid userId)
	{
		return _context
			.UserClaims
			.Where(x => x.UserId == userId)
			.Include(x => x.User)
			.ToList();
	}

	public List<UserExternal> GetUserExternalsByUserName(string userName)
	{
		return _context
			.UserClaims
			.Where(x => x.User.UserName == userName)
			.Include(x => x.User)
			.ToList();
	}

	public async Task<UserExternal?> GetByUserIdAsync(Guid userId, string provider)
	{
		return await _context
			.UserClaims
			.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider);
	}

	public UserExternal? GetByUserId(Guid userId, string provider)
	{
		return _context
			.UserClaims
			.Include(x => x.User)
			.FirstOrDefault(x => x.UserId == userId && x.Provider == provider);
	}

	public UserExternal? GetByExternalId(string externalId, string provider)
	{
		return _context
			.UserClaims
			.Include(x => x.User)
			.FirstOrDefault(x => x.ExternalId == externalId && x.Provider == provider);
	}

	public Task<UserExternal?> GetByExternalIdAsync(string externalId, string provider)
	{
		return _context
			.UserClaims
			.Include(x => x.User)
			.FirstOrDefaultAsync(x => x.ExternalId == externalId && x.Provider == provider);
	}

	public void RemoveUserExternal(UserExternal userExternal)
	{
		RemoveBase(userExternal);
	}
}