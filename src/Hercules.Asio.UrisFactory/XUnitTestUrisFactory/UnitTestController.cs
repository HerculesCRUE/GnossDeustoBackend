// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test de los controladores
using Hercules.Asio.UrisFactory.Models.Services;
using UrisFactory.Models.Services;
using Xunit;

namespace XUnitTestUrisFactory
{
    public class UnitTestController
    {
        [Fact]
        public async void TestUriPublicationOK()
        {
            ConfigService configService = new ConfigService(ConfigService.GetBuildConfiguration());
            ConfigJsonHandler configJsonHandler = new ConfigJsonHandler(configService);
            var parsedJson = configJsonHandler.GetUrisConfig();

            Assert.True(parsedJson != null);
        }
    }
}
