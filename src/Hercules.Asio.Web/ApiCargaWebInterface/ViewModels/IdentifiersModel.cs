using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.Asio.Web.ViewModels
{
    public class IdentifiersModel
    {
        [Required]
        public Guid RepositoryId { get; set; }
        public List<string> listaSets { get; set; }
        public List<string> listaMetadataFormats { get; set; }
        public string NameRepository { get; set; }
    }
}
