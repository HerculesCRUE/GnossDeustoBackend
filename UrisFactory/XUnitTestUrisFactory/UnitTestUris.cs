using System;
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
            string uri = uriFormer.GetURI("Researcher", queryString);
            string uriResultante = "http://data.um.es/res/researcher/123d";

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

        [Trait("Priority", "1")]
        [Fact]
        public void TestUriPublicationOK()
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            queryString.Add("identifier", "123d");
            queryString.Add("sector", "sector1");
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            UriFormer uriFormer = new UriFormer(configJsonHandler.GetUrisConfig());
            string uri = uriFormer.GetURI("publication", queryString);
            string uriResultante = "http://data.um.es/res/sector1/publicacion/123d";
            Assert.True(uriResultante.Equals(uri));
        }
    }
}
