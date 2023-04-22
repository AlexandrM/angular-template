namespace AngularTemplate.Data.Models.Users;

public class UserExternal
{
	public int Id { get; set; }
	public DateTime CreatedOn { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
	public string ExternalId { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string Provider { get; set; } = null!;
}