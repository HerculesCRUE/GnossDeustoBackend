using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.ViewModels
{
    public class PublishRepositoryModel
    {
        [Required]
        public Guid RepositoryId { get; set; }
        [Required]
        public string Id { get; set; }
        [Required]
        public string Type { get; set; }
        public string Result { get; set; }
        public List<ShapeConfigViewModel> RepositoryShapes { get; set; }
    }
}
