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
        PrestamoServicio prestamoServicio,
        ServicioBase<TipoPersona> servicioTipoPersona
    ) : Controller
    {
        // GET: Usuarios
        public async Task<IActionResult> Index(string? filtro)
        {
            var usuarios = await service.ObtenerTodosAsync(filtro);
            return View(usuarios);
        }

        // GET: Usuarios/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);

            if (usuario == null)
                return NotFound();

            usuario.Cedula = usuario.Cedula;

            return View(usuario);
        }

        // GET: Usuarios/Crear
        public async Task<IActionResult> Crear()
        {
            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View();
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                estudiante.Cedula = estudiante.Cedula.Trim().Replace("-","");
                await service.AgregarAsync(estudiante);
                return RedirectToAction(nameof(Index));
            }

            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(estudiante);
        }

        // GET: Usuarios/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);
            if (usuario == null)
                return NotFound();

            var tipos = await servicioTipoPersona.ObtenerTodosAsync();

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(usuario);
        }

        // POST: Usuarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Estudiante usuario)
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

            ViewData["TiposPersonas"] = new SelectList(tipos, "CodigoTipo", "NombreTipo");
            return View(usuario);
        }

        // GET: Usuarios/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await service.ObtenerPorIdAsync(id.Value);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Estudiantes/{id}/Prestamos")]
        public async Task<IActionResult> PrestamosPorEstudiante(int? id, string? filtro)
        {
            if (id == null)
                return NotFound();

            var estudiante = await service.ObtenerPorIdAsync(id.Value);
            if (estudiante == null)
                return NotFound();

            var prestamos = await prestamoServicio.ObtenerPorIdEstudianteAsync(estudiante.CodigoEstudiante, filtro);

            var prestamosVM = prestamos
                .OrderByDescending(p => p.FechaPrestamo)
                .Select(p => new PrestamoVM
                {
                    CodigoPrestamo = p.CodigoPrestamo,
                    FechaPrestamo = p.FechaPrestamo,
                    FechaDevolucion = p.FechaDevolucion,
                    FechaDevolucionEsperada = p.FechaDevolucionEsperada,
                    Estudiante = p.Estudiante.Nombre,
                    Libro = p.Libro.Titulo,
                })
                .ToList() ?? new List<PrestamoVM>();

            ViewBag.Title = "Prestamos del estudiante " + estudiante.NumeroCarnet;
            ViewBag.CodigoEstudiante = estudiante.CodigoEstudiante;
            ViewBag.Filtro = filtro;

            return View(prestamosVM);
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
                CodigoTipo = estudiante.CodigoTipo
            };

            var prestamosActivos = estudiante.Prestamos
                .Where(p => !(p.Eliminado ?? false))
                .Where(p => p.FechaDevolucion == null)
                .ToList();

            var prestamoVencido = prestamosActivos
                .Any(p => p.FechaDevolucionEsperada < DateOnly.FromDateTime(DateTime.Now));

            estudianteVM.PuedeTomarPrestamos = prestamosActivos.Count < 3 && !prestamoVencido;
            estudianteVM.ErrorPrestamo = prestamoVencido
                ? "El estudiante tiene un préstamo vencido. No puede tomar más préstamos hasta que se resuelva."
                : "El estudiante ha alcanzado el límite de 3 préstamos activos.";

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
                CodigoTipo = estudiante.CodigoTipo
            };

            var prestamosActivos = estudiante.Prestamos
                .Where(p => !(p.Eliminado ?? false))
                .Where(p => p.FechaDevolucion == null)
                .ToList();

            var prestamoVencido = prestamosActivos
                .Any(p => p.FechaDevolucionEsperada < DateOnly.FromDateTime(DateTime.Now));

            estudianteVM.PuedeTomarPrestamos = prestamosActivos.Count < 3 && !prestamoVencido;
            estudianteVM.ErrorPrestamo = prestamoVencido
                ? "El estudiante tiene un préstamo vencido. No puede tomar más préstamos hasta que se resuelva."
                : "El estudiante ha alcanzado el límite de 3 préstamos activos.";

            return Json(estudianteVM);
        }

        private bool UsuarioExists(int id)
        {
            return context.Estudiantes.Any(u => u.CodigoEstudiante == id);
        }
    }
}
