using API_CARGA.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_CARGA.Models.Services
{
    public class SyncsConfigBDService : ISyncsConfigService
    {
        private readonly EntityContext _context;
        public SyncsConfigBDService(EntityContext context)
        {
            _context = context;
        }

        public List<SyncConfig> GetSyncConfigs()
        {
            return _context.SyncConfig.OrderBy(sync => sync.Name).ToList();
        }

        public SyncConfig GetSyncConfigById(Guid id)
        {
            return _context.SyncConfig.FirstOrDefault(sync => sync.SyncConfigID.Equals(id));
        }

        public bool RemoveSyncConfig(Guid identifier)
        {
            try
            {
                SyncConfig sincConfig = GetSyncConfigById(identifier);
                if (sincConfig != null)
                {
                    _context.Entry(sincConfig).State = EntityState.Deleted;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Guid AddSyncConfig(SyncConfig syncConfig)
        {
            Guid syncConfigID = Guid.Empty;
            //if (GetRepositoryConfigByName(repositoryConfig.Name) == null)
            //{
            syncConfigID = Guid.NewGuid();
            syncConfig.SyncConfigID = syncConfigID;
            _context.SyncConfig.Add(syncConfig);
            _context.SaveChanges();
            //}
            return syncConfigID;
        }

        public bool ModifySyncConfig(SyncConfig syncConfig)
        {
            bool modified = false;
            SyncConfig syncConfigOriginal = GetSyncConfigById(syncConfig.SyncConfigID);
            if (syncConfigOriginal != null)
            {
                //CheckDataExceptions(repositoryConfigOriginal, repositoryConfig);
                syncConfigOriginal.Name = syncConfig.Name;
                syncConfigOriginal.StartHour = syncConfig.StartHour;
                syncConfigOriginal.UpdateFrequency = syncConfig.UpdateFrequency;
                syncConfigOriginal.RepositoryIdentifier = syncConfig.RepositoryIdentifier;
                syncConfigOriginal.RepositorySetIdentifiers = syncConfig.RepositorySetIdentifiers;
                _context.SaveChanges();
                modified = true;

            }
            return modified;
        }
    }
}
