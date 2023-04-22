namespace AngularTemplate.Data.Repositories.Users;

using Models.Users;

public interface IUserExternalRepository
{
	Task<int> AddUserExternalAsync(UserExternal external);
	int AddUserExternal(UserExternal external);
	Task<int> AddUserExternalsAsync(IEnumerable<UserExternal> claims);

	Task<List<UserExternal>> GetUserExternalsByUserIdAsync(Guid userId);

	Task<UserExternal?> GetByUserIdAsync(Guid userId, string provider);
	UserExternal? GetByUserId(Guid userId, string provider);

	UserExternal? GetByExternalId(string externalId, string provider);
	Task<UserExternal?> GetByExternalIdAsync(string externalId, string provider);

	void RemoveUserExternal(UserExternal userExternal);
}