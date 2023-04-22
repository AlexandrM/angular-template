namespace AngularTemplate.Shared;

using Microsoft.Extensions.Configuration;

public static class SharedConfiguration
{
	public static IConfiguration ConfigurationContainer { get; private set; }

	public static IConfiguration CreateConfigurationContainer()
	{
		try
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			if (string.IsNullOrWhiteSpace(environment))
				return ConfigurationContainer = new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile($"appsettings.shared.json").Build();

			return ConfigurationContainer = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile($"appsettings.shared.{environment}.json").Build();
		}
		catch (Exception ex)
		{
			throw;
		}
	}
}