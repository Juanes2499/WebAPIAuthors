using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAPIAuthors.Controllers;
using WebAPIAuthors.Servicios;

namespace WebAPIAuthors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            //var AutoresController = new AutoresController(new ApplicationDbContext(null), new servicioB()); //Princiop de inyecci�n de dependencia, como IServicio es abstracto podemos esoger cualquier m�todo de la interface IServicio
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
