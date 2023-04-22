namespace AngularTemplate.Web.Identity;

using Data.Repositories.Users;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Security.Claims;

public class ProfileService : IProfileService
{
	private readonly IUserRepository _userRepository;

	public ProfileService(
		IUserRepository userRepository
	)
	{
		_userRepository = userRepository;
	}

	public async Task GetProfileDataAsync(ProfileDataRequestContext context)
	{
		var sub = context.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
		if (sub != null && Guid.TryParse(sub.Value, out var id))
		{
			var user = await _userRepository.GetUserAsync(id);
			if (user != null)
				if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Email))
					context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, user.Email));
		}
	}

	public Task IsActiveAsync(IsActiveContext context)
	{
		return Task.FromResult(0);
	}
}