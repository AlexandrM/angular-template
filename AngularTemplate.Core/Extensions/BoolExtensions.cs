namespace AngularTemplate.Core.Extensions;

public static class BoolExtensions
{
	public static string ToLowerCase(this bool value)
	{
		return value.ToString().ToLower();
	}

	public static string? ToLowerCase(this bool? value)
	{
		return value == null ? null : value.ToString()?.ToLower();
	}
}