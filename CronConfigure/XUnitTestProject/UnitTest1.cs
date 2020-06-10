// Copyright (c) UTE GNOSS - UNIVERSIDAD DE DEUSTO
// Licenciado bajo la licencia GPL 3. Ver https://www.gnu.org/licenses/gpl-3.0.html
// Proyecto Hércules ASIO Backend SGI. Ver https://www.um.es/web/hercules/proyectos/asio
// Test
using CronConfigure.Controllers;
using CronConfigure.Models.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void TestControllerJob()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            JobController controller = new JobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(), "07/05/2022 12:36", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestControllerJobWithoutDate()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            JobController controller = new JobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(), null);
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }
        [Fact]
        public void TestControllerJobBadGuid()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            JobController controller = new JobController(cron, methodService);
            var result = controller.AddExecution("1231-11ef-12", "07/05/2022 12:36", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("identificador invalido"));
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestControllerJobBadParams()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            JobController controller = new JobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(), "07/05/2022 12:36", "07/05/2001 12:36", null,"12");
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("falta el tipo de objeto"));
            }
            else
            {
                Assert.True(false);
            }
        }


        [Fact]
        public void TestControllerRecurringJob()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            RecurringJobController controller = new RecurringJobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(),"prueba_job", "07/05/2022 12:36", "*/15 * * * *", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestControllerRecurringJobBadCron()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            RecurringJobController controller = new RecurringJobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(), "prueba_job", "07/05/2022 12:36", "*/15 * * *", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("invalid cron expression"));
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestControllerRecurringJobExistName()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            RecurringJobController controller = new RecurringJobController(cron, methodService);
            var result = controller.AddExecution(Guid.NewGuid().ToString(), "0", "07/05/2022 12:36", " */ 15 * * * *", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("Ya existe una tarea con ese nombre"));
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestControllerScheduledJob()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            ScheduledJobController controller = new ScheduledJobController(cron, methodService);
            var result = controller.AddScheduledJob("07/05/2021 12:36", Guid.NewGuid().ToString(), "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void TestControllerScheduledJobFailWithoutDate()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            ScheduledJobController controller = new ScheduledJobController(cron, methodService);
            var result = controller.AddScheduledJob(null, Guid.NewGuid().ToString());
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("La fecha de ejecución es obligatoria"));
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestControllerScheduledJobFailDate()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            ScheduledJobController controller = new ScheduledJobController(cron, methodService);
            var result = controller.AddScheduledJob("07/25/2001 12:36", Guid.NewGuid().ToString());
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("fecha de ejecución inválida"));
            }
            else
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestControllerScheduledJobBadGuid()
        {
            ICronApiService cron = new MockCronApiService();
            IProgramingMethodService methodService = new MockProgramingMethodService();
            ScheduledJobController controller = new ScheduledJobController(cron, methodService);
            var result = controller.AddScheduledJob("07/05/2021 12:36", "1321-e", "07/05/2001 12:36");
            if (result is BadRequestObjectResult)
            {
                Assert.True(((BadRequestObjectResult)result).Value.Equals("identificador invalido"));
            }
            else
            {
                Assert.True(false);
            }
        }
    }
}
