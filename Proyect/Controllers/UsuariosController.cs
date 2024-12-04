
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyect.Models;
using Proyect.Validaciones.ValidacionesLuis; // Importar las validaciones

namespace Proyect.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ProyectContext _context;

        public UsuariosController(ProyectContext context)
        {
            _context = context;
            
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var proyectContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            return View(await proyectContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol");
            return View();
        }

        // POST: Usuarios/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,Estado,FechaCreacion,IdRol")] Usuario usuario)
        {
            // Verificar si el rol está activo
            var rol = _context.Roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
            if (rol != null && !rol.Activo)
            {
                ModelState.AddModelError("IdRol", "El rol seleccionado está inactivo y no puede ser asignado.");
            }

            // Validación manual con el validador
            var validator = new UsuarioValidator(_context);
            var validationResult = validator.Validate(usuario);

            if (!validationResult.IsValid)
            {

                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }


            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }



        // GET: Usuarios/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        // POST: Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IdUsuario,TipoDocumento,Documento,Nombre,Apellido,Celular,Direccion,CorreoElectronico,Estado,FechaCreacion,IdRol")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            // Verificar si el rol está activo
            var rol = _context.Roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
            if (rol != null && !rol.Activo)
            {
                ModelState.AddModelError("IdRol", "El rol seleccionado está inactivo y no puede ser asignado.");
            }

            // Validación manual con el validador
            var validator = new UsuarioValidator(_context);
            var validationResult = validator.Validate(usuario);

            if (!validationResult.IsValid)
            {

                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // No se actualiza el documento, ya que es "quemado"
                    var existingUser = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == id);
                    usuario.Documento = existingUser.Documento; // Mantener el documento sin cambios

                    _context.Update(usuario);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == usuario.IdUsuario))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }


            ViewBag.IdRol = new SelectList(_context.Roles, "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }


        // POST: Usuarios/ActualizarEstado
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int id, bool estado)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = estado;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok(); // Responde con éxito
            }
            return BadRequest(); // En caso de error
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}


