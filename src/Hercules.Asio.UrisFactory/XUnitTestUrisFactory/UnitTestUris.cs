// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test unitarios de la creación de uris
using System.Collections.Generic;
using UrisFactory.Extra.Exceptions;
using UrisFactory.Models.Services;
using Xunit;

namespace XUnitTestUrisFactory
{
    public class UnitTestUris
    {
        [Fact]
        public void TestUriResearcherOK()
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI("AcademicDegree", queryString);
            string uriResultante = "http://graph.um.es/res/academic-degree/123d";

            Assert.True(uriResultante.Equals(uri));
        }

        [Fact]
        public void TestUriRdfTypeOK()
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI("gradoOtorgado", queryString, true);
            string uriResultante = "http://graph.um.es/res/awarded-degree/123d";

            Assert.True(uriResultante.Equals(uri));
        }

        //[Fact]
        //public void TestUriResearcherFail()
        //{
        //    Dictionary<string, string> queryString = new Dictionary<string, string>();
        //    queryString.Add("identifier", "123d");
        //    ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
        //    UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
        //    Assert.Throws<ParametersNotConfiguredException>(() => uriFormer.GetURI("resourc", "researcher", queryString));
        //}
        [Fact]
        public void TestUriResearcherFailClass()
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            Assert.Throws<ParametersNotConfiguredException>(() => uriFormer.GetURI("resea", queryString));
        }

        [Fact]
        public void TestUriPublicationFail()
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            Assert.Throws<ParametersNotConfiguredException>(() => uriFormer.GetURI("publication", queryString));
        }
    }
}
