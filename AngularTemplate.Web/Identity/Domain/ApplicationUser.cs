namespace AngularTemplate.Web.Identity.Domain;

public class ApplicationUser<TKey>
{
#pragma warning disable CS8618 // Non-nullable property 'Id' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
	public TKey Id { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'Id' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
	public string UserName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string PasswordHash { get; set; } = null!;

	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? FullName { get; set; }
	public bool IsEmailConfirmed { get; set; }
}