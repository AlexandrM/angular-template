﻿namespace AngularTemplate.Web.Identity.Repositories;

using Domain;
using Microsoft.AspNetCore.Identity;

public class RoleStore : IRoleStore<ApplicationRole>
{
	public void Dispose()
	{
	}

	public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}