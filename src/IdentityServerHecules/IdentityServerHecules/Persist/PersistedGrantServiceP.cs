using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHecules.Persist
{
    public class PersistedGrantServiceP : IPersistedGrantServiceP
    {
        PersistedGrantDbContext context;
        public PersistedGrantServiceP(IServiceScopeFactory iserviceScope)
        {
            var serviceScope = iserviceScope.CreateScope();
            context = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        }

        public void Add(PersistedGrant persistedGrant)
        {
            context.PersistedGrants.Add(persistedGrant);
            context.SaveChanges();
        }

        public PersistedGrant Get(string key)
        {
            return context.PersistedGrants.FirstOrDefault(item => item.Key.Equals(key));
        }

        public IEnumerable<PersistedGrant> GetAll(string subjectId)
        {
            return context.PersistedGrants.Where(item => item.SubjectId.Equals(subjectId)).ToList();
        }

        public IEnumerable<PersistedGrant> GetAll(string subjectId, string clientId)
        {
            return context.PersistedGrants.Where(item => item.SubjectId.Equals(subjectId) && item.ClientId.Equals(clientId)).ToList();
        }

        public IEnumerable<PersistedGrant> GetAll(string subjectId, string clientId, string type)
        {
            return context.PersistedGrants.Where(item => item.SubjectId.Equals(subjectId) && item.ClientId.Equals(clientId) && item.Type.Equals(type)).ToList();
        }

        public void Remove(PersistedGrant persistedGrant)
        {
            context.Entry(persistedGrant).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            context.SaveChanges();
        }

        public void RemoveAll(IEnumerable<PersistedGrant> persistedGrants)
        {
            foreach (var persistedGrant in persistedGrants)
            {
                context.Entry(persistedGrant).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            context.SaveChanges();
        }

        public void Update(PersistedGrant existing)
        {
            var persistedGrant = Get(existing.Key);
            persistedGrant.ClientId = existing.ClientId;
            persistedGrant.CreationTime = existing.CreationTime;
            persistedGrant.Data = existing.Data;
            persistedGrant.Expiration = existing.Expiration;
            persistedGrant.SubjectId = existing.SubjectId;
            persistedGrant.Type = existing.Type;
            context.SaveChanges();
        }
    }
}
