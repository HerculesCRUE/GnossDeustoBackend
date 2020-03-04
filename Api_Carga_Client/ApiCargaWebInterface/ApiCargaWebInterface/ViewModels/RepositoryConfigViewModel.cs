using System;
using System.ComponentModel.DataAnnotations;


namespace ApiCargaWebInterface.ViewModels
{
    [Display(Name = "Config repository")]
    public class RepositoryConfigViewModel
    {
        [Display(Name = "Identifier")]
        public Guid RepositoryConfigID { get; set; }

        public string Name { get; set; }

        [Display(Name = "Oauth token")]
        public string OauthToken { get; set; }

        public string Url { get; set; }
    }
}
