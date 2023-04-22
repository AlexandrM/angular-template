namespace AngularTemplate.Data.Repositories.Users;

using AngularTemplate.Data.Models.Users;
using Repositories;
using Microsoft.EntityFrameworkCore;

public class UserRepository : AppRepository<User>, IUserRepository
{
	public async Task<User?> GetUserAsync(Guid id)
	{
		return await GetAsync(id);
	}

	public async Task<User?> GetByUserUserNameAsync(string userName)
	{
		return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
	}

	public User? GetByUserUserName(string userName)
	{
		return _context.Users.FirstOrDefault(x => x.UserName == userName);
	}

	public async Task<int> AddUserAsync(User user)
	{
		return await AddAsync(user);
	}

	public async Task<int> UpdateUserAsync(User user)
	{
		return await UpdateAsync(user);
	}
}