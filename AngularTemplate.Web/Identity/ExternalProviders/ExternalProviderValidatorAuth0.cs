namespace AngularTemplate.Web.Identity.ExternalProviders;

using Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Core.Extensions;
using Core.Identity;

public class ExternalProviderValidatorAuth0 : IExternalProviderValidator
{
	private readonly ExternalProvidersConfig _externalProvidersConfig;
	private readonly ILogger<ExternalProviderValidatorAuth0> _logger;

	public ExternalProviderValidatorAuth0(
		ILogger<ExternalProviderValidatorAuth0> logger,
		IOptions<ExternalProvidersConfig> externalProvidersConfig
	)
	{
		_externalProvidersConfig = externalProvidersConfig.Value;
		_logger = logger;

		Provider = ExternalProvidersConst.PROVIDER_AUTH0;
		IsConfigured = _externalProvidersConfig.IsAuth0Configured();
	}

	public string Provider { get; }
	public bool IsConfigured { get; }

	public async Task<ExternalProviderValidatorResult> ValidateAsync(string token)
	{
		var auth0Domain = _externalProvidersConfig.Auth0!.Domain!;
		var secretKey = _externalProvidersConfig.Auth0!.SecretKey!;
		var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
		var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
			$"{auth0Domain}.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
		var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

		var validations = new TokenValidationParameters
		{
			ValidIssuer = auth0Domain,
			ValidAudiences = new[] { _externalProvidersConfig.Auth0!.ClientId },
			IssuerSigningKeys = openIdConfig.SigningKeys,
			TokenDecryptionKey = securityKey
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var user = tokenHandler.ValidateToken(token, validations, out var validatedToken);
		if (user.Identity?.IsAuthenticated != true)
		{
			return ExternalProviderValidatorResult.Failed();
		}

		var email = user.Claims.GetEmail();
		var emailVerified = user.Claims.GetValue("email_verified").AsBool();
		var fullName = user.Claims.GetValue("name")?.ToString();
		var externalId = user.Claims.GetSID();

		if (email == null || externalId == null)
		{
			return ExternalProviderValidatorResult.Failed();
		}

		return ExternalProviderValidatorResult.Success(email, emailVerified, externalId, fullName);
	}
}