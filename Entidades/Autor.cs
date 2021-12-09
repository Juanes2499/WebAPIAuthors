using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using WebAPIAuthors.Validaciones;

namespace WebAPIAuthors.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        //[StringLength(maximumLength: 50, MinimumLength = 5, ErrorMessage = "El campo {0} debe contener entre {2} y {1} caracteres")]
        //[PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        [NotMapped]
        public int Menor { get; set; }
        [NotMapped]
        public int Mayor { get; set; }
        public List<Libro> Libros { get; set; } //Propiedad de navegación de tipo Lista de Libro llamada Libros para poderse relacionar, es decir que un autor puede tener muchos libros

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Cuando se quiere poner reglas de validación por modelo no se debe poner la otras validaciones por que hará que no salgan todas la reglas de modelo
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primerLetra = Nombre[0].ToString();
                if(primerLetra != primerLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] { nameof(Nombre) });
                }
            }

            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser más grande que el campor Mayor", new string[] { nameof(Menor) });
            }
        }
    }
}
