using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Entities
{
    public class PageInfo
    {
        public Guid PageId { get; set; }
        public string Route { get; set; }
        public string Content { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime? LastRequested { get; set; }
    }
}
