namespace AngularTemplate.Core.Identity;

using System;
using System.Diagnostics.Contracts;

public interface IExternalProviderValidator
{
	string Provider { get; }
	public bool IsConfigured { get; }

	Task<ExternalProviderValidatorResult> ValidateAsync(string token);
}

public class ExternalProviderValidatorResult
{
	public static ExternalProviderValidatorResult Failed()
	{
		return new ExternalProviderValidatorResult();
	}

	public static ExternalProviderValidatorResult Success(string email, bool emailVerified, string externalId,
		string? fullName)
	{
		return new ExternalProviderValidatorResult(email, emailVerified, externalId, fullName);
	}

	public ExternalProviderValidatorResult()
	{
		IsSuccessed = false;
	}

	public ExternalProviderValidatorResult(string email, bool emailVerified, string externalId, string? fullName) :
		this()
	{
		Email = email;
		EmailVerified = emailVerified;
		IsSuccessed = true;
		ExternalId = externalId;
		FullName = fullName;
	}

	public bool IsSuccessed { get; }
	public string? Email { get; }
	public bool EmailVerified { get; }
	public string? ExternalId { get; }
	public string? FullName { get; }
}