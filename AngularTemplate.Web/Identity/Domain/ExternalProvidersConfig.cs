namespace AngularTemplate.Web.Identity.Domain;

public class ExternalProvidersConfig
{
	public class Auth0Config
	{
		public string? ClientId { get; set; }
		public string? Domain { get; set; }
		public string? SecretKey { get; set; }
	}

	public class GoogleConfig
	{
		public string? ClientId { get; set; }
	}

	public Auth0Config? Auth0 { get; set; }
	public GoogleConfig? Google { get; set; }
}