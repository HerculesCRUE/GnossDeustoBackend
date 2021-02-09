using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHecules.Persist
{
    public interface IPersistedGrantServiceP
    {
        void Add(PersistedGrant persistedGrant);

        void Update(PersistedGrant existing);

        PersistedGrant Get(string key);

        IEnumerable<PersistedGrant> GetAll(string subjectId);

        IEnumerable<PersistedGrant> GetAll(string subjectId, string clientId);

        IEnumerable<PersistedGrant> GetAll(string subjectId, string clientId, string type);

        void Remove(PersistedGrant persistedGrant);

        void RemoveAll(IEnumerable<PersistedGrant> persistedGrants);
    }
}
