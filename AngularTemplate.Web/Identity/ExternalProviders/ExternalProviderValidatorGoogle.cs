namespace AngularTemplate.Web.Identity.ExternalProviders;

using AngularTemplate.Core.Identity;
using Domain;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

public class ExternalProviderValidatorGoogle : IExternalProviderValidator
{
	private readonly ExternalProvidersConfig _externalProvidersConfig;

	public ExternalProviderValidatorGoogle(
		IOptions<ExternalProvidersConfig> externalProvidersConfig
	)
	{
		_externalProvidersConfig = externalProvidersConfig.Value;

		Provider = ExternalProvidersConst.PROVIDER_GOOGLE;
		IsConfigured = _externalProvidersConfig.IsGoogleConfigured();
	}

	public string Provider { get; }
	public bool IsConfigured { get; }

	public async Task<ExternalProviderValidatorResult> ValidateAsync(string token)
	{
		var settings = new GoogleJsonWebSignature.ValidationSettings()
		{
			Audience = new List<string>() { _externalProvidersConfig.Google!.ClientId! }
		};
		var result = await GoogleJsonWebSignature.ValidateAsync(token, settings);
		if (!result.EmailVerified) return ExternalProviderValidatorResult.Failed();

		return ExternalProviderValidatorResult.Success(result.Email, result.EmailVerified, result.Subject, result.Name);
	}
}