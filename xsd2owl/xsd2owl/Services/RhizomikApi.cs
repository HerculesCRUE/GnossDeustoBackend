using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace xsd2owl.Services
{
    public static class RhizomikApi
    {
        private static string urlApi = "http://rhizomik.net/redefer-services/xsd2owl";
        public static async Task<ContentResult> GetResultRhizomik(string uriXsd)
        {
            HttpClient client = new HttpClient();
            string url = $"{urlApi}?xsd={uriXsd}";
            HttpResponseMessage response = await client.GetAsync(url);
            string result = "";
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
                return new ContentResult
                {
                    Content = result,
                    ContentType = "application/xml",
                    StatusCode = 200
                };
            }
            else
            {
                return new ContentResult
                {
                    Content = $"Check that the xsd file is valid or check that {uriXsd} is an accessible path",
                    StatusCode = 400
                };
            }

        }
    }
}
