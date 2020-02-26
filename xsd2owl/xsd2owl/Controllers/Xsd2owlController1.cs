using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using xsd2owl.Services;

namespace xsd2owl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Xsd2owlController : ControllerBase
    {

        [HttpGet]
        public async Task<ContentResult> Get(string uriXsd)
        {
            return await RhizomikApi.GetResultRhizomik(uriXsd);
        }

        [HttpPost]
        public async Task<ContentResult> Post(IFormFile formFile)
        {
            string extension = Path.GetExtension(formFile.FileName);
            string urlHost = $"{Request.Scheme}://{Request.Host.Value}";
            if (formFile != null && formFile.Length > 0)
            {
                
                string urlFile = FilesUtilities.SaveFormFile(urlHost, formFile).Result;

                return await RhizomikApi.GetResultRhizomik(urlFile);
            }
            else
            {
                return new ContentResult
                {
                    Content = "Empty file",
                    StatusCode = 400
                };
            }
        }

       
    }
}