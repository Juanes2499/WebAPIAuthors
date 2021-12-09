using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAuthors.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute //Para poder realizar validaciones hay que heredar de ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Aqui se crea la lógica de validación
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primerLetra = value.ToString()[0].ToString();

            if(primerLetra != primerLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
