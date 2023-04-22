namespace AngularTemplate.Web.Identity.Configure;

using AngularTemplate.Web.Infrastructure;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Options;

public class ConfigureIdentityServerOptions : IConfigureNamedOptions<IdentityServerAuthenticationOptions>
{
	readonly GlobalSettings _globalSettings;
	readonly ILogger<ConfigureIdentityServerOptions> _logger;

	public ConfigureIdentityServerOptions(
		GlobalSettings globalSettings,
		ILogger<ConfigureIdentityServerOptions> logger
	)
	{
		_globalSettings = globalSettings;
		_logger = logger;
	}

	public void Configure(string? name, IdentityServerAuthenticationOptions options)
	{
		//_logger.LogError($"Configure: {_globalSettings.Host}");
		if (name == IdentityServerAuthenticationDefaults.AuthenticationScheme)
		{
			options.RequireHttpsMetadata = false;
			options.ApiName = IdentityServerConfig.ApiName;
			options.Authority = _globalSettings.Host;
		}
	}

	// This won't be called, but is required for the IConfigureNamedOptions interface
	public void Configure(IdentityServerAuthenticationOptions options) => Configure(Options.DefaultName, options);
}