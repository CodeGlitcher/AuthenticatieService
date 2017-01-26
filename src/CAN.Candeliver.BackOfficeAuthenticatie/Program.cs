using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Threading;

namespace CAN.Candeliver.BackOfficeAuthenticatie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Wait for the docker container to startup
            Thread.Sleep(60000);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
