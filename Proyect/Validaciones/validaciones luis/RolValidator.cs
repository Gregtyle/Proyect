using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;

namespace Proyect.Validaciones.ValidacionesLuis
{
    public class RolValidator : AbstractValidator<Role>
    {
            public RolValidator(IEnumerable<string> nombresExistentes)
            {
                RuleFor(x => x.NombreRol)
                    .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                    .MaximumLength(50).WithMessage("El nombre del rol no puede exceder los 50 caracteres.")
                    .Must(nombre => !nombresExistentes.Contains(nombre))
                    .WithMessage("El nombre del rol ya existe.")
                    .When(x => x.IdRol == 0); // Solo validar el nombre si es un rol nuevo
      

                RuleFor(x => x.Descripcion)
                    .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");
            }
 
        private async Task<bool> NombreRolExistente(string nombre, CancellationToken cancellationToken)
        {
            using (var context = new ProyectContext())
            {
                return await context.Roles.AnyAsync(r => r.NombreRol == nombre, cancellationToken);
            }
        }
    }
}
