namespace AngularTemplate.Web.Identity.Repositories;

using System.Security.Cryptography;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class PasswordHasher : IPasswordHasher<ApplicationUser<Guid>>
{
	public string HashPassword(ApplicationUser<Guid> user, string password)
	{
		byte[] salt;
		salt = RandomNumberGenerator.GetBytes(16);

		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
		var hash = pbkdf2.GetBytes(20);

		var hashBytes = new byte[36];
		Array.Copy(salt, 0, hashBytes, 0, 16);
		Array.Copy(hash, 0, hashBytes, 16, 20);

		var passwordHash = Convert.ToBase64String(hashBytes);

		return passwordHash;
	}

	private bool IsValidHash(string password, string passwordHash)
	{
		/* Fetch the stored value */
		/* Extract the bytes */
		var hashBytes = Convert.FromBase64String(passwordHash);
		/* Get the salt */
		var salt = new byte[16];
		Array.Copy(hashBytes, 0, salt, 0, 16);
		/* Compute the hash on the password the user entered */
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
		var hash = pbkdf2.GetBytes(20);
		/* Compare the results */
		for (var i = 0; i < 20; i++)
			if (hashBytes[i + 16] != hash[i])
				return false;

		return true;
	}

	public PasswordVerificationResult VerifyHashedPassword(
		ApplicationUser<Guid> user,
		string hashedPassword,
		string providedPassword
	)
	{
		if (IsValidHash(providedPassword, hashedPassword)) return PasswordVerificationResult.Success;

		return PasswordVerificationResult.Failed;
	}
}

public class AppUserManager : UserManager<ApplicationUser<Guid>>
{
	public AppUserManager(
		IUserStore<ApplicationUser<Guid>> store,
		IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<ApplicationUser<Guid>> passwordHasher,
		IEnumerable<IUserValidator<ApplicationUser<Guid>>> userValidators,
		IEnumerable<IPasswordValidator<ApplicationUser<Guid>>> passwordValidators,
		ILookupNormalizer keyNormalizer,
		IdentityErrorDescriber errors,
		IServiceProvider services,
		ILogger<UserManager<ApplicationUser<Guid>>> logger) :
		base(
			store,
			optionsAccessor,
			passwordHasher,
			userValidators,
			passwordValidators,
			keyNormalizer,
			errors,
			services,
			logger
		)
	{
	}
}