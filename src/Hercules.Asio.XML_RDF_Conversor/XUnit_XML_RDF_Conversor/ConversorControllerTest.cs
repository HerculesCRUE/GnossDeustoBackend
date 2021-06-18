using Hercules.Asio.XML_RDF_Conversor.Controllers;
using Hercules.Asio.XML_RDF_Conversor.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace XUnit_XML_RDF_Conversor
{
    public class ConversorControllerTest
    {
        [Fact]
        public void TestConfigurationFiles()
        {
            ConfigUrlService configUrlService = new ConfigUrlService(ConfigUrlService.GetBuildConfiguration());
            CallUrisFactoryApiService callUrisFactoryApiService = new CallUrisFactoryApiService(new CallApiService(), null, configUrlService);
            ConversorController conversorController = new ConversorController(null, callUrisFactoryApiService);
            List<string> listResult = conversorController.ConfigurationFilesList();
            Assert.True(listResult.Contains("oai_cerif_openaire") && listResult.Contains("XML_ASIO"));
        }

        [Fact]
        public void TestConvert()
        {
            ConfigUrlService configUrlService = new ConfigUrlService(ConfigUrlService.GetBuildConfiguration());
            CallUrisFactoryApiService callUrisFactoryApiService = new CallUrisFactoryApiService(new CallApiService(), null, configUrlService);
            ConversorController conversorController = new ConversorController(null, callUrisFactoryApiService);
            string xml = File.ReadAllText("Documents/xml.txt");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(xml);
            writer.Flush();
            stream.Position = 0;
            var file = new FormFile(stream, 0, stream.Length, null, "xml.txt");
            var resultAction = conversorController.Convert(file, "oai_cerif_openaire");


            var result = (FileContentResult)resultAction;
            Assert.True(result.FileContents.Length > 700);
        }
    }
}
