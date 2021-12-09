using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //Me permite usar UseSqlServer
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPIAuthors.Filtros;
using WebAPIAuthors.Middlewares;
using WebAPIAuthors.Servicios;

namespace WebAPIAuthors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllers();
            //En el servicio de addcontroles de configura el servicio global
            services.AddControllers(opciones => {
                    opciones.Filters.Add(typeof(FiltroDeExcepcion));
                }    
            ).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Para evitar ciclos infinitos entre las entidades que estan relacionadas, en .NET Core 6 es IgnoreCycles

            /*
             * ApplicationDbContext es configurado como un servicio, cada vez que el ApplicationDbContext aparezca
             * como una dependencia de una clase (constuctor) el sistema de inyección de dependencias se va a encargar
             * de instanciar correctamente el ApplicationDbContext con todas sus configuraciones*/
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))); //Se agrega la base de datos que ya fue configurada en el appsetings.json

            /*Creación de un servicio para que pueda ser instanciado desde cualquier clase en el constructor -> sistema
             de inyección de dependencia.*/
            /*Transient: nos va a dar siempre una nueva instancia de la clase servicioA sin importar que este en el mismo
             contexto*/
            services.AddTransient<IServicio, servicioA>(); //Interfaz y su clase//Le estoy diciendo al sistema de inyección de dependencia que cuando una clase requiera un IServicio entonces pásales un ServicioA (Una instancia de la clase ServicioA), además resueve las demás dependecia de la clases, en este caso el serviciA resolverá todo lo de ILogger
            services.AddTransient<ServicioTransient>(); //Una clase como servicio
            /*Scoped: la vida del servicio aumenta, la instancia de la clase es siempre la misma durante el mismo contexto*/
            services.AddScoped<ServicioScoped>();
            /*Scoped: la vida del servicio aumenta, la instancia de la clase es siempre la misma durante el mismo contexto*/
            services.AddSingleton<ServicioSingleton>();

            //Se añade los servicios para el filtro de response caching
            services.AddResponseCaching();

            //Servicio de autenticación 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            //Se añade al sistema de inyección de dependecias el filtro personalizado
            services.AddTransient<MiFiltroDeAccion>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API de Autores", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //Middleware para interceptar las peticiones y evitar que sigan, es decir se retorna desde aquí
            //app.Run(async context => {
            //    await context.Response.WriteAsync("De aqui no puede pasar la petición");
            //});

            //app.Use(async (conetxto, siguiente) => { 
            //    using(var ms = new MemoryStream())
            //    {
            //        var cuerpoOriginalRespuesta = conetxto.Response.Body;
            //        conetxto.Response.Body = ms;
            //        await siguiente.Invoke();

            //        ms.Seek(0, SeekOrigin.Begin);
            //        string respuesta = new StreamReader(ms).ReadToEnd();
            //        await ms.CopyToAsync(cuerpoOriginalRespuesta);
            //        conetxto.Response.Body = cuerpoOriginalRespuesta;

            //        logger.LogInformation(respuesta);
            //    }
            //});

            //Middleware de una clase creada por nosostros
            app.useLogearRespuestaHTTP();

            //Middleware - Birfurcación
            app.Map("/Birfurcacion", app =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Birfurcacion - segunda ruta");
                });
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAuthors v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            //Filtro de cache
            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
