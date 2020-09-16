// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Contexto de la base de datos

using API_DISCOVER.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_DISCOVER.Models
{
    public class EntityContext : DbContext
    {
        private string _defaultSchema;
        public DbSet<DiscoverItem> DiscoverItem { get; set; }
        public EntityContext(DbContextOptions options, bool memory = false)
            : base(options)
        {
            if (!memory)
            {
                Database.Migrate();
            }
           

        }

    }
}
