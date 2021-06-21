using Microsoft.AspNetCore.Mvc;
using OAI_PMH.Controllers;
using OAI_PMH_CVN.Models.Services;
using System;
using Xunit;

namespace TestProjectCVN2OAI_PMH
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            ConfigOAI_PMH_CVN configOAI_PMH_CVN = new ConfigOAI_PMH_CVN(ConfigOAI_PMH_CVN.GetBuildConfiguration());
            OAI_PMHController oAI_PMHController = new OAI_PMHController(configOAI_PMH_CVN);
            var resultAction = oAI_PMHController.Get(OaiPmhNet.OaiVerb.Identify);
            var result = (FileContentResult)resultAction;
            Assert.True(result.FileContents.Length > 0);
        }
    }
}
