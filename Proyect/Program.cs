using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Validaciones;
using Proyect.Validaciones.ValidacionesLuis;


namespace Proyect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agrega el DbContext al contenedor de servicios
            builder.Services.AddDbContext<ProyectContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectConnection")));

            // Registrar validadores de FluentValidation de toda la aplicaci�n
            builder.Services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UsuarioValidator>());


            // Add services to the container.

            // Registrar validadores espec�ficos si es necesario
            builder.Services.AddScoped<IValidator<Usuario>, UsuarioValidator>();
            builder.Services.AddScoped<IValidator<Role>, RolValidator>();
            builder.Services.AddScoped<IValidator<Permiso>, PermisoValidator>();

            // Agregar servicios necesarios para la aplicaci�n
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuraci�n de la tuber�a de solicitudes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // Configuraci�n para entornos de producci�n
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Configuraci�n de rutas para controladores MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}

