namespace AngularTemplate.Web.Controllers;

using System.Security.Authentication;
using Core.Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.Models.IdentityResources;

public class BaseApiController : ControllerBase
{
	public Guid UserId
	{
		get
		{
			var subject = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
			if (subject.IsPresent() && Guid.TryParse(subject, out var guid)) return guid;

			throw new AuthenticationException("Get UserId failed");
		}
	}

	public ObjectResult ProblemBadRequest(string? detail = null)
	{
		return Problem(detail, statusCode: StatusCodes.Status400BadRequest);
	}
}