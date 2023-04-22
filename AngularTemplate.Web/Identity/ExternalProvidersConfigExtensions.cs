using AngularTemplate.Web.Identity.Domain;

namespace AngularTemplate.Web.Identity;

using Core.Extensions;

public static class ExternalProvidersConfigExtensions
{
	public static bool IsGoogleConfigured(this ExternalProvidersConfig? config)
	{
		return config != null
		       && config.Google != null
		       && config.Google.ClientId.IsPresent();
	}

	public static bool IsAuth0Configured(this ExternalProvidersConfig? config)
	{
		return config != null
		       && config.Auth0 != null
		       && config.Auth0.ClientId.IsPresent()
		       && config.Auth0.Domain.IsPresent()
		       && config.Auth0.SecretKey.IsPresent();
	}
}