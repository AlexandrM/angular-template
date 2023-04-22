namespace AngularTemplate.Web.Identity;

using Core.Extensions;
using IdentityServer4.Services;
using Infrastructure;

public class CorsPolicyService : ICorsPolicyService
{
	private readonly ILogger<CorsPolicyService> _logger;
	private readonly GlobalSettings _globalSettings;

	public CorsPolicyService(
		ILogger<CorsPolicyService> logger,
		GlobalSettings globalSettings
	)
	{
		_logger = logger;
		_globalSettings = globalSettings;
	}

	public Task<bool> IsOriginAllowedAsync(string origin)
	{

		var result = origin.EqualsIgnoreCase(_globalSettings.Host);
		return Task.FromResult(result);
	}
}
