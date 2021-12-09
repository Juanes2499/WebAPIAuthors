using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAuthors.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; } //Propiedad de navegación de tipo Autor llamada Autor para poderse relacionar, es decir que un libro puede tener un autor
    }
}
