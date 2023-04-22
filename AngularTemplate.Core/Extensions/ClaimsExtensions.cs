namespace AngularTemplate.Core.Extensions;

using System.Security.Claims;

public static class ClaimsExtensions
{
	public static string? GetValue(this IEnumerable<Claim> claims, string type)
	{
		return claims.FirstOrDefault(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase))?.Value;
	}

	public static string? GetValue(this IEnumerable<Claim> claims, string[] types)
	{
		return claims.FirstOrDefault(x => types.Any(z => x.Type.Equals(z, StringComparison.OrdinalIgnoreCase)))?.Value;
	}

	public static string? GetEmail(this IEnumerable<Claim> claims)
	{
		return GetValue(claims, new[] { ClaimTypes.Email, "email" });
	}

	public static string? GetNameIdentifier(this IEnumerable<Claim> claims)
	{
		return GetValue(claims, ClaimTypes.NameIdentifier);
	}

	public static string? GetSID(this IEnumerable<Claim> claims)
	{
		return GetValue(claims, "sid");
	}
}