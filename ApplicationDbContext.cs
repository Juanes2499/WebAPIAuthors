using Microsoft.EntityFrameworkCore; //Me permite heredar DbContext
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAPIAuthors.Entidades;

namespace WebAPIAuthors
{
    public class ApplicationDbContext : DbContext
    {
        //ctr + . me permite crear el constructor de manera sencilla
        //Contructor para configurar lo relacionaod con la base de datos 
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //Se crea las tablas
        public DbSet<Autor> Autores { get; set; } //Cree una tabla a partir del esquema/entidad/propiedades de Autor y que la tabla se llame Autores
        public DbSet<Libro> Libros { get; set; } //Cree una tabla a partir del esquema/entidad/propiedades de Libro y que la tabla se llame Libros
    }
}
