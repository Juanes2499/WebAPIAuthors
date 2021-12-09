using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAPIAuthors.Entidades;

namespace WebAPIAuthors.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {

        private readonly ApplicationDbContext context;
        public LibrosController(ApplicationDbContext ContextDb) //Constructor
        {
            this.context = ContextDb;
        }

        [HttpGet("{id:int}")]
        public async Task<List<Libro>> Get(int id)
        {
            //Para este método se de usar ActionResult en la declaración de la función
            //Este metodo me genera un problema de overlooping, que se arregla con la línea 34 del startup.cs
            //En .NetCore 6 en la misma línea 34 del startup.cs se remplaza al final .preserve por .IgnoreCycle
            // return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);

            //Para este método se debe usar List en la declaración de la función
            var librosList = await (
                    from a in context.Libros
                    join b in context.Autores on a.AutorId equals b.Id
                    where b.Id == id
                    select new Libro()
                    {
                        Id = a.Id,
                        Titulo = a.Titulo,
                        AutorId = a.AutorId,
                        Autor = b
                    }
                ).ToListAsync().ConfigureAwait(false);

            return librosList;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                return BadRequest($"No existe el autor de Id: {libro.AutorId}");
            }
            else
            {
                context.Add(libro);
                await context.SaveChangesAsync();
                return Ok();
            }
        }
    }
}
