using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHecules.Persist
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantServiceP persistedGrantService;

        public PersistedGrantStore(IPersistedGrantServiceP persistedGrantService)
        {
            this.persistedGrantService = persistedGrantService;
        }
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrants = this.persistedGrantService.GetAll(subjectId).ToList();

            var model = persistedGrants.Select(x => x.ToModel());

            return Task.FromResult(model);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = this.persistedGrantService.Get(key);
            var model = persistedGrant?.ToModel();

            return Task.FromResult(model);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = this.persistedGrantService.GetAll(subjectId, clientId);

            this.persistedGrantService.RemoveAll(persistedGrants);
            
            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = this.persistedGrantService.GetAll(subjectId, clientId, type);

            this.persistedGrantService.RemoveAll(persistedGrants);


            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            var persistedGrant = this.persistedGrantService.Get(key);

            if (persistedGrant != null)
            {
                this.persistedGrantService.Remove(persistedGrant);
            }

            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            var existing = this.persistedGrantService.Get(grant.Key);
            if(existing == null)
            {
                var persistedGrant = grant.ToEntity();
                this.persistedGrantService.Add(persistedGrant);
            }
            else
            {
                this.persistedGrantService.Update(existing);
            }
            return Task.FromResult(0);
        }
    }
}
