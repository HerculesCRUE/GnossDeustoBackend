using Hercules.Asio.CVN2OAI_PMH.Models.Services;
using Microsoft.AspNetCore.Mvc;
using OAI_PMH.Controllers;
using OAI_PMH_CVN.Models.Services;
using System;
using System.IO;
using System.Xml;
using Xunit;

namespace TestProjectCVN2OAI_PMH
{
    public class UnitTestController
    {
        [Fact]
        public void TestIdentify()
        {
            ConfigOAI_PMH_CVN configOAI_PMH_CVN = new ConfigOAI_PMH_CVN(ConfigOAI_PMH_CVN.GetBuildConfiguration());
            OAI_PMHController oAI_PMHController = new OAI_PMHController(configOAI_PMH_CVN, null);
            var resultAction = oAI_PMHController.Get(OaiPmhNet.OaiVerb.Identify);
            var result = (FileContentResult)resultAction;
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(result.FileContents);
            doc.Load(ms);
            var identifyNode = doc.DocumentElement.ChildNodes.Item(2);
            Assert.True(identifyNode.Name.Equals("Identify"));
        }

        [Fact]
        public void TestListMetaData()
        {
            ConfigOAI_PMH_CVN configOAI_PMH_CVN = new ConfigOAI_PMH_CVN(ConfigOAI_PMH_CVN.GetBuildConfiguration());
            OAI_PMHController oAI_PMHController = new OAI_PMHController(configOAI_PMH_CVN, null);
            var resultAction = oAI_PMHController.Get(OaiPmhNet.OaiVerb.ListMetadataFormats);
            var result = (FileContentResult)resultAction;
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(result.FileContents);
            doc.Load(ms);
            var identifyNode = doc.DocumentElement.ChildNodes.Item(2);
            Assert.True(identifyNode.Name.Equals("ListMetadataFormats"));
        }
        [Fact]
        public void TestListIndetifiers()
        {
            ConfigOAI_PMH_CVN configOAI_PMH_CVN = new ConfigOAI_PMH_CVN(ConfigOAI_PMH_CVN.GetBuildConfiguration());
            UtilMock util = new UtilMock();
            OAI_PMHController oAI_PMHController = new OAI_PMHController(configOAI_PMH_CVN, util);
            var resultAction = oAI_PMHController.Get(OaiPmhNet.OaiVerb.ListIdentifiers, metadataPrefix: "rdf");
            var result = (FileContentResult)resultAction;
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(result.FileContents);
            doc.Load(ms);
            var identifyNode = doc.DocumentElement.ChildNodes.Item(2);
            Assert.True(identifyNode.Name.Equals("ListIdentifiers"));
        }

        [Fact]
        public void TestListSets()
        {
            ConfigOAI_PMH_CVN configOAI_PMH_CVN = new ConfigOAI_PMH_CVN(ConfigOAI_PMH_CVN.GetBuildConfiguration());
            OAI_PMHController oAI_PMHController = new OAI_PMHController(configOAI_PMH_CVN, null);
            var resultAction = oAI_PMHController.Get(OaiPmhNet.OaiVerb.ListSets);
            var result = (FileContentResult)resultAction;
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(result.FileContents);
            doc.Load(ms);
            var identifyNode = doc.DocumentElement.ChildNodes.Item(2);
            Assert.True(identifyNode.Name.Equals("ListSets"));
        }
    }
}
