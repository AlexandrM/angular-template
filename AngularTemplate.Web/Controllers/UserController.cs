namespace AngularTemplate.Web.Controllers;

using Models.Auth;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using AngularTemplate.Data.Models.Users;
using Core.Extensions;
using AngularTemplate.Core.Identity;
using Core;
using Services.Users;
using Services.Users.Domain;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseApiController
{
	private readonly UserManager<ApplicationUser<Guid>> _userManager;
	private readonly UserService _userService;
	private readonly IEnumerable<IExternalProviderValidator> _externalProviderValidators;

	public UserController(
		UserManager<ApplicationUser<Guid>> userManager,
		UserService userService,
		IEnumerable<IExternalProviderValidator> externalProviderValidators
	)
	{
		_userManager = userManager;
		_userService = userService;
		_externalProviderValidators = externalProviderValidators;
	}

	[HttpPost("~/api/user/register")]
	public async Task<object> Register([FromBody] RegisterModel request)
	{
		var user = new ApplicationUser<Guid>
		{
			Id = Guid.NewGuid(),
			UserName = request.UserName,
			Email = request.UserName
		};

		var result = await _userManager.CreateAsync(user, request.Password);

		return new
		{
			result.Succeeded,
			Errors = result.Errors.Where(x => !x.Code.Equals("DuplicateUserName"))
		};
	}

	[Authorize]
	[HttpGet("~/api/user/profile")]
	public async Task<object> ProfileGet()
	{
		var user = await _userManager.GetUserAsync(User);

		return user.ToModel(await _userService.GetUserExternals(user.Id));
	}

	[Authorize]
	[HttpPost("~/api/user/profile")]
	public async Task<object> ProfileUpdate([FromBody] ProfileModel model)
	{
		var user = await _userManager.GetUserAsync(User);

		user.FirstName = model.FirstName;
		user.LastName = model.LastName;
		user.FullName = model.FullName.NullIfEmpty() ?? $"{model.FirstName} {model.LastName}";

		await _userManager.UpdateAsync(user);

		return user.ToModel(await _userService.GetUserExternals(user.Id));
	}

	[Authorize]
	[HttpPost("~/api/user/attach")]
	public async Task<object> AttachProvider([FromBody] AttachModel model)
	{
		var validator = _externalProviderValidators.FirstOrDefault(x => x.Provider.EqualsIgnoreCase(model.Provider));
		if (validator == null) return ProblemBadRequest("Provider not supported");

		var result = await validator.ValidateAsync(model.Token);
		if (!result.IsSuccessed) return ProblemBadRequest("Token not valid");

		var execResult = _userService.AddUserExternal(UserId, model.Provider, result.ExternalId!, result.Email!);
		switch (execResult.Status)
		{
			case AddUserExternalStatusEnum.success:
				return Ok(new
				{
					success = true
				});

			case AddUserExternalStatusEnum.externalid_used:
			case AddUserExternalStatusEnum.user_not_valid:
				return ProblemBadRequest("Provider already connected");

			default:
				throw new NotSupportedException(execResult.Status.ToString());
		}
	}

	[Authorize]
	[HttpPost("~/api/user/deattach")]
	public async Task<object> DeattachProvider([FromBody] DeattachModel model)
	{
		var exec = await _userService.RemoveUserExternal(UserId, model.Provider);

		if (exec.Status == GeneralExecStatus.not_found) return ProblemBadRequest("Provider not connected");

		return Ok(new
		{
			success = true
		});
	}
}

public class DeattachModel
{
	[Required] public string Provider { get; set; } = null!;
}

public class AttachModel
{
	[Required] public string Provider { get; set; } = null!;
	[Required] public string Token { get; set; } = null!;
}

public class ProfileModel
{
	public class ProviderModel
	{
		public string Provider { get; set; } = null!;

		public DateTime CreatedOn { get; set; }
	}

	public string? Email { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? FullName { get; set; }
	public bool? IsEmailConfirmed { get; set; }

	public List<ProviderModel>? Providers { get; set; }
}

public static class ProfileModelExtensions
{
	public static ProfileModel ToModel(this ApplicationUser<Guid> user, List<UserExternal> userClaims)
	{
		return new ProfileModel
		{
			Email = user.Email,
			FirstName = user.FirstName,
			LastName = user.LastName,
			FullName = user.FullName,
			IsEmailConfirmed = user.IsEmailConfirmed,
			Providers = userClaims?.Select(x => new ProfileModel.ProviderModel
			{
				Provider = x.Provider,
				CreatedOn = x.CreatedOn
			}).ToList()
		};
	}
}