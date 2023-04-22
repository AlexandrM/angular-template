namespace AngularTemplate.Core.Extensions;

public static class StringExtensions
{
	public static bool IsMissing(this string? str)
	{
		return str == null || string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str.Trim());
	}

	public static bool IsPresent(this string? str)
	{
		return !IsMissing(str);
	}

	public static string? NullIfEmpty(this string? str)
	{
		return IsMissing(str) ? null : str;
	}

	public static bool EqualsIgnoreCase(this string? str, string? value)
	{
		return str != null && str.Equals(value, StringComparison.OrdinalIgnoreCase);
	}

	public static bool AsBool(this string? str, bool defaultValue = false)
	{
		return true.ToString().Equals(str, StringComparison.OrdinalIgnoreCase) || defaultValue;
	}
}