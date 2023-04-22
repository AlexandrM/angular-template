namespace AngularTemplate.Core.Extensions;

public static class GuidExtensions
{
	public static bool EqualsString(this Guid guid, string str)
	{
		return guid.ToString().EqualsIgnoreCase(str);
	}
}