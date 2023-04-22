namespace AngularTemplate.Web.Models.Auth;

public class RegisterModel
{
	public string UserName { get; set; } = null!;
	public string Password { get; set; } = null!;
	public string ConfirmPassword { get; set; } = null!;
}