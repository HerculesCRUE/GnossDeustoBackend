using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UrisFactory.Controllers;
using UrisFactory.Middlewares;
using UrisFactory.Models.ConfigEntities;
using UrisFactory.Models.Services;
using Xunit;

namespace XUnitTestUrisFactory
{
    public class UnitTestController
    {
        [Fact]
        public async void TestUriPublicationOK()
        {
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler();
            var parsedJson = configJsonHandler.GetUrisConfig();

            Assert.True(parsedJson != null);
        }
    }
}
