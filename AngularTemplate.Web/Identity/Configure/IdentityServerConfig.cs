namespace AngularTemplate.Web.Identity.Configure;

using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

public class IdentityServerConfig
{
	public static class ClaimConstants
	{
		public const string Subject = "subject";
		public const string Permission = "permission";
	}

	public static class ScopeConstants
	{
		public const string Roles = "roles";
	}

	public const string ApiName = "api";
	public const string ApiFriendlyName = "AngularTemplate.Web API";
	public const string AppClientID = "angulartemplate_spa";

	public static IEnumerable<IdentityResource> GetIdentityResources()
	{
		return new List<IdentityResource>
		{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile(),
			new IdentityResources.Phone(),
			new IdentityResources.Email(),
			new(ScopeConstants.Roles, new List<string> { JwtClaimTypes.Role })
		};
	}

	public static IEnumerable<ApiScope> GetApiScopes()
	{
		return new List<ApiScope>
		{
			new(ApiName, ApiFriendlyName)
			{
				UserClaims =
				{
					JwtClaimTypes.Name,
					JwtClaimTypes.Email,
					JwtClaimTypes.PhoneNumber,
					JwtClaimTypes.Role,
					ClaimConstants.Permission
				}
			}
		};
	}

	public static IEnumerable<ApiResource> GetApiResources()
	{
		return new List<ApiResource>
		{
			new(ApiName)
			{
				Scopes = { ApiName }
			}
		};
	}

	public static IEnumerable<Client> GetClients()
	{
		return new List<Client>
		{
			new()
			{
				ClientId = AppClientID,
				ClientSecrets = new List<Secret>
				{
					new Secret("secret".Sha256())
				},
				AllowedGrantTypes = new[]
				{
					GrantType.AuthorizationCode,
					GrantType.ClientCredentials,
					GrantType.DeviceFlow,
					GrantType.ResourceOwnerPassword,
					"external"
				},
				AllowAccessTokensViaBrowser = true,
				RequireClientSecret = false,

				RedirectUris = new[]
				{
					"http://localhost:4200/silent-refresh.html",
				},

				AllowedScopes =
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					IdentityServerConstants.StandardScopes.Phone,
					IdentityServerConstants.StandardScopes.Email,
					ScopeConstants.Roles,
					ApiName
				},
				AllowOfflineAccess = true,
				RefreshTokenExpiration = TokenExpiration.Sliding,
				RefreshTokenUsage = TokenUsage.OneTimeOnly
			}
		};
	}
}