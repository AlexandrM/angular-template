namespace AngularTemplate.Web.Identity.Repositories;

using Data.Models.Users;
using Data.Repositories.Users;
using Domain;
using Microsoft.AspNetCore.Identity;

public class UserStore :
	IUserStore<ApplicationUser<Guid>>,
	IUserPasswordStore<ApplicationUser<Guid>>,
	IUserEmailStore<ApplicationUser<Guid>>
{
	private readonly IUserRepository _userRepository;

	public UserStore(
		IUserRepository userRepository
	)
	{
		_userRepository = userRepository;
	}

	public void Dispose()
	{
	}

	public Task<string> GetUserIdAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Id.ToString());
	}

	public Task<string> GetUserNameAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.UserName);
	}

	public Task SetUserNameAsync(ApplicationUser<Guid> user, string userName, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> GetNormalizedUserNameAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task SetNormalizedUserNameAsync(ApplicationUser<Guid> user, string normalizedName,
		CancellationToken cancellationToken)
	{
		user.UserName = normalizedName;
		return Task.FromResult(0);
	}

	public async Task<IdentityResult> CreateAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		var result = await _userRepository.AddUserAsync(new User
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			PasswordHash = user.PasswordHash,
			FirstName = user.FirstName,
			LastName = user.LastName,
			FullName = user.FullName,
			IsEmailConfirmed = user.IsEmailConfirmed
		});

		return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
	}

	public async Task<IdentityResult> UpdateAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		var userDb = new User
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			PasswordHash = user.PasswordHash,
			FirstName = user.FirstName,
			LastName = user.LastName,
			FullName = user.FullName,
			IsEmailConfirmed = user.IsEmailConfirmed
		};

		var result = await _userRepository.UpdateUserAsync(userDb);
		return result == 1 ? IdentityResult.Success : IdentityResult.Failed();
	}

	public Task<IdentityResult> DeleteAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public async Task<ApplicationUser<Guid>> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetUserAsync(Guid.Parse(userId));
		if (user == null)
		{
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}

		return new ApplicationUser<Guid>
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			PasswordHash = user.PasswordHash,
			FirstName = user.FirstName,
			LastName = user.LastName,
			FullName = user.FullName,
			IsEmailConfirmed = user.IsEmailConfirmed
		};
	}

	public async Task<ApplicationUser<Guid>> FindByNameAsync(string normalizedUserName,
		CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByUserUserNameAsync(normalizedUserName);
		if (user == null)
		{
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}

		return new ApplicationUser<Guid>
		{
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			PasswordHash = user.PasswordHash,
			FirstName = user.FirstName,
			LastName = user.LastName,
			FullName = user.FullName,
			IsEmailConfirmed = user.IsEmailConfirmed
		};
	}

	public Task SetPasswordHashAsync(ApplicationUser<Guid> user, string passwordHash,
		CancellationToken cancellationToken)
	{
		user.PasswordHash = passwordHash;
		return Task.FromResult(0);
	}

	public Task<string> GetPasswordHashAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PasswordHash);
	}

	public Task<bool> HasPasswordAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task SetEmailAsync(ApplicationUser<Guid> user, string email, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> GetEmailAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Email);
	}

	public Task<bool> GetEmailConfirmedAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task SetEmailConfirmedAsync(ApplicationUser<Guid> user, bool confirmed, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<ApplicationUser<Guid>> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
	{
		return FindByNameAsync(normalizedEmail, cancellationToken);
	}

	public Task<string?> GetNormalizedEmailAsync(ApplicationUser<Guid> user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user?.Email);
	}

	public Task SetNormalizedEmailAsync(ApplicationUser<Guid> user, string normalizedEmail,
		CancellationToken cancellationToken)
	{
		return Task.FromResult(0);
	}
}