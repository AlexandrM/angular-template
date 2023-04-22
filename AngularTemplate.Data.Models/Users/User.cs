namespace AngularTemplate.Data.Models.Users;

public class User
{
	public Guid Id { get; set; }
	public string UserName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string PasswordHash { get; set; } = null!;
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? FullName { get; set; }
	public string? Phone { get; set; }
	public bool IsEmailConfirmed { get; set; }
	public IEnumerable<UserExternal>? UserClaims { get; set; }
}