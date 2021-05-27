using Hercules_SAML.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Hercules_SAML.Models
{
    public class EntityContext : DbContext
    {
        private string _defaultSchema;
        public DbSet<TokenSAML> TokenSAML { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
