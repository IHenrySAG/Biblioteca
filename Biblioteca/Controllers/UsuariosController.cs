using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN))]
    public class UsuariosController(
        ContextoBiblioteca context,
        UsuarioServicio service,
        ServicioBase<TipoPersona> servicioTipoPersona
    ) : Controller
    {
        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await service.ObtenerTodosAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            if (tipos.Count == 0)
                return RedirectToAction("Create", "TipoPersonas", new { RedirectedFrom = "CreateUsuario" });

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoUsuario,Nombre,Apellido,Cedula,NumeroCarnet,CodigoTipo,Estado")] Estudiante usuario)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(usuario);
                return RedirectToAction(nameof(Index));
            }

            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            if (tipos.Count == 0)
                return RedirectToAction("Create", "TipoPersonas", new { RedirectedFrom = "CreateUsuario" });

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);
            if (usuario == null)
                return NotFound();

            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            if (tipos.Count == 0)
                return RedirectToAction("Create", "TipoPersonas", new { RedirectedFrom = "EditUsuario" });

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoUsuario,Nombre,Apellido,Cedula,NumeroCarnet,CodigoTipo,Estado")] Estudiante usuario)
        {
            if (id != usuario.CodigoEstudiante)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(usuario);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.CodigoEstudiante))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            if (tipos.Count == 0)
                return RedirectToAction("Create", "TipoPersonas", new { RedirectedFrom = "EditUsuario" });

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return context.Estudiantes.Any(u => u.CodigoEstudiante == id);
        }
    }
}
