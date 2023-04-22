namespace AngularTemplate.Services.Users;

using Domain;
using Core;
using Data.Models.Users;
using Data.Repositories.Users;

public class UserService
{
	private readonly IUserRepository _userRepository;
	private readonly IUserExternalRepository _userExternalRepository;

	public UserService(
		IUserRepository userRepository,
		IUserExternalRepository userExternalRepository
	)
	{
		_userRepository = userRepository;
		_userExternalRepository = userExternalRepository;
	}

	public async Task<User?> Get(Guid id)
	{
		return await _userRepository.GetUserAsync(id);
	}

	public async Task<List<UserExternal>> GetUserExternals(Guid userId)
	{
		return await _userExternalRepository.GetUserExternalsByUserIdAsync(userId);
	}

	public async Task<UserExternal?> GetUserExternal(Guid userId, string provider)
	{
		return await _userExternalRepository.GetByUserIdAsync(userId, provider);
	}

	public async Task<Exec<UserExternal>> RemoveUserExternal(Guid userId, string provider)
	{
		var result = new Exec<UserExternal>();

		var userExternal = await _userExternalRepository.GetByUserIdAsync(userId, provider);
		if (userExternal == null) return result.Set(GeneralExecStatus.not_found);

		_userExternalRepository.RemoveUserExternal(userExternal);

		return result.Set(userExternal);
	}

	private readonly object _lock = new();

	public Exec<UserExternal, AddUserExternalStatusEnum> AddUserExternal(
		Guid userId,
		string provider,
		string externalId,
		string email
	)
	{
		var result = new Exec<UserExternal, AddUserExternalStatusEnum>(AddUserExternalStatusEnum.success);

		var externalLogin = _userExternalRepository.GetByUserId(userId, provider);

		if (externalLogin != null) return result.Set(externalLogin);

		lock (_lock)
		{
			externalLogin = _userExternalRepository.GetByUserId(userId, provider);
			// externalLogin exists
			if (externalLogin != null) return result.Set(externalLogin);

			externalLogin = _userExternalRepository.GetByExternalId(externalId, provider);
			// externalId connected to another user
			if (externalLogin != null) return result.Set(AddUserExternalStatusEnum.externalid_used);

			var user = _userRepository.GetByUserUserName(email);
			// there are another user with that email
			if (user?.Id != userId) return result.Set(AddUserExternalStatusEnum.user_not_valid);

			externalLogin = new UserExternal
			{
				UserId = userId,
				CreatedOn = DateTime.UtcNow,
				Provider = provider.ToLower(),
				ExternalId = externalId,
				Email = email
			};
			_userExternalRepository.AddUserExternal(externalLogin);
		}

		return result;
	}
}