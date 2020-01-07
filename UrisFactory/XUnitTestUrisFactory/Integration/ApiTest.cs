using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace XUnitTestUrisFactory
{
    public class ApiTest
    {
        
        public async void CallApiResourceOK()
        {
            HttpClient client = new HttpClient();
            string url= "https://localhost:44353/Uris?resource=resource&resource_class=researcher&identifier=123ewd&additionalProp1=string&additionalProp2=string&additionalProp3=string";
            HttpResponseMessage response = await client.GetAsync(url);
            string uri="";
            if (response.IsSuccessStatusCode)
            {
                uri = await response.Content.ReadAsStringAsync();
            }
            bool success = response.IsSuccessStatusCode && uri.Equals("http://datos.um.es/res/investigador/123ewd");
            Assert.True(success);
        }

        
        public async void CallApiResourceFail()
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44353/Uris?resource=resource&resource_class=researche&identifier=123ewd&additionalProp1=string&additionalProp2=string&additionalProp3=string";
            HttpResponseMessage response = await client.GetAsync(url);
            
            Assert.True(!response.IsSuccessStatusCode);
        }

        
        public async void CallApiPublicationOK()
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44353/Uris?resource=resource&resource_class=publication&identifier=123ewd&additionalProp1=string&additionalProp2=string&additionalProp3=string&sector=cap1";
            HttpResponseMessage response = await client.GetAsync(url);
            string uri = "";
            if (response.IsSuccessStatusCode)
            {
                uri = await response.Content.ReadAsStringAsync();
            }
            bool success = response.IsSuccessStatusCode && uri.Equals("http://datos.um.es/res/cap1/publicacion/123ewd");
            Assert.True(success);
        }

        
        public async void CallApiPublicationFail()
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44353/Uris?resource=resource&resource_class=publication&identifier=123ewd&additionalProp1=string&additionalProp2=string&additionalProp3=string";
            HttpResponseMessage response = await client.GetAsync(url);

            Assert.True(!response.IsSuccessStatusCode);
        }
    }
}
