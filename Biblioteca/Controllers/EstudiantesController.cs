using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.BIBLIOTECARIO))]
    public class EstudiantesController(
        ContextoBiblioteca context,
        EstudiantesServicio service,
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

            if (!tipos.Any())
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

            if (!tipos.Any())
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

            if (!tipos.Any())
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

            if (!tipos.Any())
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

        [HttpGet]
        public async Task<JsonResult> BuscarEstudianteCarnetJson(string documento)
        {
            var estudiante = await service.ObtenerPorCarnetAsync(documento);

            if(estudiante is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(null);
            }

            var estudianteVM = new EstudianteVM
            {
                CodigoEstudiante = estudiante.CodigoEstudiante,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                Cedula = estudiante.Cedula,
                NumeroCarnet = estudiante.NumeroCarnet,
                CodigoTipo = estudiante.CodigoTipo,
                PrestamoActivo = estudiante.Prestamos
                    .Where(p => !(p.Eliminado ?? false))
                    .Select(p=>p.Libro)
                    .FirstOrDefault() ?? null
            };

            return Json(estudianteVM);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarEstudianteCedulaJson(string documento)
        {
            var estudiante = await service.ObtenerPorCedulaAsync(documento);

            if (estudiante is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(null);
            }

            var estudianteVM = new EstudianteVM
            {
                CodigoEstudiante = estudiante.CodigoEstudiante,
                Nombre = estudiante.Nombre,
                Apellido = estudiante.Apellido,
                Cedula = estudiante.Cedula,
                NumeroCarnet = estudiante.NumeroCarnet,
                CodigoTipo = estudiante.CodigoTipo,
                PrestamoActivo = estudiante.Prestamos
                    .Where(p => !(p.Eliminado ?? false))
                    .Select(p => p.Libro)
                    .FirstOrDefault() ?? null
            };

            return Json(estudianteVM);
        }

        private bool UsuarioExists(int id)
        {
            return context.Estudiantes.Any(u => u.CodigoEstudiante == id);
        }
    }
}
