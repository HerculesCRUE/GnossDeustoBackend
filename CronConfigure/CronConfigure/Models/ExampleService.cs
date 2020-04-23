using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronConfigure.Models
{
    public static class ExampleService
    {
        public static EjemploResultado WriteLine(string fichero)
        {
            if (!System.IO.File.Exists(fichero))
            { 
                System.IO.File.Create(fichero);
            }
            System.IO.File.AppendAllText(fichero, DateTime.Now.ToString() + "\n");
            return new EjemploResultado()
            {
                Codigo = 22,
                Error = "Correcto"
            };
        }

        public static void PonerEnCola(string nombreCron, string nombreFichero, string cronExpression, bool executeInmediatly = false)
        {
            RecurringJob.AddOrUpdate(nombreCron, () => ExampleService.WriteLine(nombreFichero), cronExpression);
            if (executeInmediatly)
            {
                RecurringJob.Trigger(nombreCron);
            }
            
        }

    }
}
