namespace AngularTemplate.Web.Identity;

using AngularTemplate.Data.Models.Users;
using Domain;
using Data.Repositories.Users;
using Repositories;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Core.Extensions;
using Core.Identity;

public class ExtensionGrantValidator : IExtensionGrantValidator
{
	private readonly IUserStore<ApplicationUser<Guid>> _userStore;
	private readonly ILogger<ExtensionGrantValidator> _logger;
	private readonly AppUserManager _appUserManager;
	private readonly IUserExternalRepository _userExternalRepository;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ExternalProvidersConfig _externalProvidersConfig;
	private readonly IEnumerable<IExternalProviderValidator> _externalProviderValidators;

	public ExtensionGrantValidator(
		ILogger<ExtensionGrantValidator> logger,
		IUserStore<ApplicationUser<Guid>> userStore,
		AppUserManager appUserManager,
		IUserExternalRepository userExternalRepository,
		IHttpContextAccessor httpContextAccessor,
		IOptions<ExternalProvidersConfig> externalProvidersConfig,
		IEnumerable<IExternalProviderValidator> externalProviderValidators
	)
	{
		if (externalProvidersConfig == null) 
			throw new ArgumentNullException(nameof(externalProvidersConfig));

		_userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
		_appUserManager = appUserManager ?? throw new ArgumentNullException(nameof(appUserManager));
		_userExternalRepository =
			userExternalRepository ?? throw new ArgumentNullException(nameof(userExternalRepository));
		_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		_externalProvidersConfig = externalProvidersConfig.Value;
		_externalProviderValidators = externalProviderValidators;
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public string GrantType => "external";

	public async Task ValidateAsync(ExtensionGrantValidationContext context)
	{
		var provider = context.Request.Raw["provider"];
		if (provider == null)
			throw new ArgumentNullException("provider not found");

		var validator = _externalProviderValidators.FirstOrDefault(x => x.Provider.EqualsIgnoreCase(provider));
		if (validator == null || !validator.IsConfigured)
		{
			context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest,
				$"Provider not supported: {provider}");
			return;
		}

		var token = context.Request.Raw["token"];
		if (token == null)
			throw new ArgumentNullException("token not found");

		var result = await validator.ValidateAsync(token);
		if (!result.IsSuccessed)
		{
			context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Token not valid");
			return;
		}

		context.Result = await ProcessAsync(result.Email!, result.ExternalId!, provider, result.EmailVerified,
			result.FullName);
	}

	private readonly object _lock = new();

	private async Task<GrantValidationResult> ProcessAsync(
		string email,
		string externlaId,
		string provider,
		bool isEmailConfirmed,
		string? fullName
	)
	{
		// connect to exists user
		if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
		{
			return new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Authentication failed");
		}
		else
		{
			var user = await _appUserManager.FindByEmailAsync(email);
			if (user == null)
			{
				if (!isEmailConfirmed)
					return new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Email not confirmed");

				user = await CreateUser(email, isEmailConfirmed, fullName);
				if (user == null) return new GrantValidationResult(TokenRequestErrors.InvalidRequest);
			}

			var claim = await _userExternalRepository.GetByUserIdAsync(user.Id, provider);
			if (claim == null) claim = AddClaim(user, email, externlaId, provider);

			return ClaimToGrantValidationResult(claim.UserId, "", "");
		}
	}

	private async Task<ApplicationUser<Guid>?> CreateUser(string email, bool isEmailConfirmed, string? fullName)
	{
		var user = new ApplicationUser<Guid>
		{
			Id = Guid.NewGuid(),
			UserName = email,
			Email = email,
			IsEmailConfirmed = isEmailConfirmed,
			FullName = fullName
		};
		var password = "Qq1!_" + Guid.NewGuid();
		var result = await _appUserManager.CreateAsync(user, password);
		if (!result.Succeeded) return null;

		return user;
	}

	private UserExternal AddClaim(ApplicationUser<Guid> user, string email, string externalId, string provider)
	{
		lock (_lock)
		{
			var claim = _userExternalRepository.GetByUserId(user.Id, provider);

			if (claim != null) return claim;

			claim = new UserExternal
			{
				UserId = user.Id,
				CreatedOn = DateTime.UtcNow,
				Provider = provider.ToLower(),
				ExternalId = externalId,
				Email = email
			};
			_userExternalRepository.AddUserExternal(claim);

			return claim;
		}
	}

	private GrantValidationResult ClaimToGrantValidationResult(Guid userId, string provider, string email)
	{
		var userIdStr = userId.ToString();
		var claims = new List<Claim>
		{
			new(JwtClaimTypes.Id, userIdStr),
			new(JwtClaimTypes.Subject, userIdStr),
			new(JwtClaimTypes.Email, email)
		};

		var grantValidationResult = new GrantValidationResult(userIdStr, provider, claims);

		return grantValidationResult;
	}
}