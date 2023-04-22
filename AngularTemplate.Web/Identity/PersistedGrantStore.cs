namespace AngularTemplate.Web.Identity;

using System.Collections.Concurrent;
using Core.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;

public class PersistedGrantStore : IPersistedGrantStore
{
	private static ConcurrentDictionary<string, PersistedGrant> _store = new ConcurrentDictionary<string, PersistedGrant>();

	public Task StoreAsync(PersistedGrant grant)
	{
		_store.AddOrUpdate(grant.Key, grant, (key, oldValue) => grant);

		return Task.CompletedTask;
	}

	public Task<PersistedGrant> GetAsync(string key)
	{
		if (!_store.TryGetValue(key, out var grant))
			throw new Exception($"Key '{key}' not found");

		return Task.FromResult(grant);
	}

	public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
	{
		var list = _store.Values.Where(x =>
			(filter.ClientId.IsMissing() || filter.ClientId == x.ClientId)
			&& (filter.SessionId.IsMissing() || filter.SessionId == x.SessionId)
			&& (filter.SubjectId.IsMissing() || filter.SubjectId == x.SubjectId)
			&& (filter.Type.IsMissing() || filter.Type == x.Type)
		);

		return Task.FromResult(list);
	}

	public Task RemoveAsync(string key)
	{
		_store.TryRemove(key, out _);

		return Task.CompletedTask;
	}

	public async Task RemoveAllAsync(PersistedGrantFilter filter)
	{
		var list = await GetAllAsync(filter);
		foreach (var item in list)
		{
			await RemoveAsync(item.Key);
		}
	}
}
