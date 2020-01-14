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
            LoadConfigJsonMiddleware loadConfigJsonMiddleware = new LoadConfigJsonMiddleware(next: (innerHttpContext) => Task.FromResult(0));
            var context = new DefaultHttpContext();
            await loadConfigJsonMiddleware.Invoke(context);
            var parsedJson = ConfigJsonHandler.GetUrisConfig();
            Assert.True(parsedJson != null);
        }
    }
}
