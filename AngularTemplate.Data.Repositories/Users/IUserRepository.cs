namespace AngularTemplate.Data.Repositories.Users;

using AngularTemplate.Data.Models.Users;

public interface IUserRepository
{
	Task<User?> GetUserAsync(Guid id);

	Task<User?> GetByUserUserNameAsync(string userName);
	User? GetByUserUserName(string userName);

	Task<int> AddUserAsync(User user);
	Task<int> UpdateUserAsync(User user);
}