using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc; //Me permite importar todo lo relacionado con el controlador
using WebAPIAuthors.Entidades; //Me importo las entidades
using Microsoft.EntityFrameworkCore;
using WebAPIAuthors.Servicios;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using WebAPIAuthors.Filtros;

namespace WebAPIAuthors.Controllers
{
    [ApiController] //Atributo (Decorador), permite hacer validaciones automáticas respecto a la data recibida en nuestro controlador
    [Route("api/autores")]
    //[Authorize] //Me protege todo el controlador
    public class AutoresController : ControllerBase //Heredamos de ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IServicio servicio; //Inyección de dependencia - Pricipio de inyección de dependecia ->IServcio es abstracto no es un tipo concreto (ServcioA o ServicioB)
        private readonly ServicioTransient servicioTransiente;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        //Constructor de la clase AutoresController
        public AutoresController(ApplicationDbContext ContextDb, IServicio servicio, ServicioTransient servicioTransiente, ServicioScoped servicioScoped, ServicioSingleton servicioSingleton, ILogger<AutoresController> logger) //Se le pasa el nombre de la clase para saber desde donde se esta generando el mensaje de log
        {
            this.context = ContextDb;
            this.servicio = servicio;
            this.servicioTransiente = servicioTransiente;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        //Tiempo de vida de los servicios
        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)] //Filtro - Se procesa la primera petición después de los 10 segundos se envía la misma petición con el fin de ahorrar recursos
        public ActionResult obteneGuids()
        {
            return Ok(new {
                AutoresController_Transient = servicioTransiente.Guid,
                ServicioA_Transient = servicio.obetnerTransient(),
                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.obetnerScoped(),
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.obetnerSingleton(),
            });
        }


        [HttpGet]
        [HttpGet("listadoAutores")]
        [HttpGet("/listadoAutres")] //Remplaza toda la base
        //[Authorize] //Solo me protege este endpoint
        [ServiceFilter(typeof(MiFiltroDeAccion))] //Filtro personalizado
        public async Task<List<Autor>> Get() {

            //throw new NotImplementedException(); //Prueba para filtro global

            logger.LogInformation("Estamos obteniendo el listado de autores");
            /*return new List<Autor>() {new Autor() {Id = 1, Nombre = "Juan Nichoy"}, new Autor() {Id = 2, Nombre = "Isabel Larrañaga"}};*/

            // Para este método se de usar ActionResult en la declaración de la función
            //Este metodo me genera un problema de overlooping, que se arregla con la línea 34 del startup.cs
            //En .NetCore 6 en la misma línea 34 del startup.cs se remplaza al final .preserve por .IgnoreCycle
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }   

        [HttpGet("FromHeader")]
        public async Task<ActionResult<Autor>> GetHeader([FromHeader] int id)
        {
            return await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            else
            {
                return autor;
            }
        }

        [HttpGet("{nombre}")]
        //Parámetros opcionales [HttpGet("{nombre}/{param2?}")]
        //Parámetros defecto en caso que sea opcional [HttpGet("{nombre}/{param2=persona}")]
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }
            else
            {
                return autor;
            }
        }

        [HttpGet("TipoEspecifico")]
        public List<Autor> GetTipoEspecifico()
        {
            return context.Autores.Include(x => x.Libros).ToList();
        }

        [HttpGet("ActionResult<T>/{id:int}")]
        public ActionResult<Autor> GetActionResult(int id)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            else
            {
                return autor;
            }
        }

        //Método POST que utilizará progrmación asíncrona
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre: {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id)
            {
                return BadRequest("El ID del autor no coincide con el ID de la URL");
            }
            else
            {
                context.Update(autor);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            else
            {
                context.Remove(new Autor() { Id = id });
                await context.SaveChangesAsync();
                return Ok();

            }
        }
    }
}
