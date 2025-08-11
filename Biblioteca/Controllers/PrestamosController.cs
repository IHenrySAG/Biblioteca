using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.BIBLIOTECARIO))]
    public class PrestamosController(
        ContextoBiblioteca context,
        PrestamoServicio service,
        ServicioBase<Empleado> servicioEmpleado,
        ServicioBase<Libro> servicioLibro,
        ServicioBase<Estudiante> servicioEstudiantes
    ) : Controller
    {
        // GET: Prestamos
        public async Task<IActionResult> Index()
        {
            var prestamos = await service.ObtenerTodosAsync();

            var prestamosVM = prestamos.Select(x => new PrestamoVM
            {
                CodigoPrestamo = x.CodigoPrestamo,
                Libro = x.Libro.Titulo,
                Estudiante = $"{x.Estudiante.Nombre} {x.Estudiante.Apellido}",
                FechaPrestamo = x.FechaPrestamo,
                FechaDevolucionEsperada = x.FechaDevolucionEsperada,
                FechaDevolucion = x.FechaDevolucion,
                MontoDia = x.MontoDia
            });
            return View(prestamosVM);
        }

        // GET: Prestamos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await service.ObtenerPorIdAsync(id.Value);

            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }

        // GET: Prestamos/Create
        public async Task<IActionResult> Create()
        {
            var libros = await servicioLibro.ObtenerTodosAsync();
            var estudiantes = await servicioEstudiantes.ObtenerTodosAsync();

            if (!libros.Any())
                ViewBag.ErrorLibros = "No hay libros registrados. Por favor, contacte con un catalogador para registrar al menos un libro antes de crear un préstamo.";

            if (!estudiantes.Any())
                ViewBag.ErrorEstudiantes = "No hay estudiantes registrados. Por favor, registre al menos un estudiante antes de crear un préstamo.";

            ViewData["Libros"] = new SelectList(libros, "CodigoLibro", "Titulo");
            ViewData["Estudiantes"] = new SelectList(estudiantes, "CodigoUsuario", "Nombre");
            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestamo prestamo)
        {
            if (!ModelState.IsValid)
            {
                return View(prestamo);
            }

            prestamo.CodigoEmpleado=HttpContext.Session.GetInt32("CodigoEmpleado") ?? 0;
            prestamo.FechaPrestamo = DateOnly.FromDateTime(DateTime.Now);
            prestamo.FechaDevolucionEsperada = prestamo.FechaPrestamo?.AddDays(7);
            await service.AgregarAsync(prestamo);

            var libro = await servicioLibro.ObtenerPorIdAsync(prestamo.CodigoLibro);
            libro!.Inventario = libro.Inventario - 1;
            await servicioLibro.ActualizarAsync(libro);

            return RedirectToAction(nameof(Index));
        }

        // GET: Prestamos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await service.ObtenerPorIdAsync(id.Value);
            if (prestamo == null)
                return NotFound();

            var empleados = await servicioEmpleado.ObtenerTodosAsync();
            var libros = await servicioLibro.ObtenerTodosAsync();
            var usuarios = await servicioEstudiantes.ObtenerTodosAsync();

            if (!empleados.Any())
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "EditPrestamo" });
            if (!libros.Any())
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "EditPrestamo" });
            if (!usuarios.Any())
                return RedirectToAction("Create", "Usuarios", new { RedirectedFrom = "EditPrestamo" });

            ViewData["Empleados"] = new SelectList(empleados, "CodigoEmpleado", "Nombre");
            ViewData["Libros"] = new SelectList(libros, "CodigoLibro", "Titulo");
            ViewData["Usuarios"] = new SelectList(usuarios, "CodigoUsuario", "Nombre");
            return View(prestamo);
        }

        // POST: Prestamos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoPrestamo,CodigoEmpleado,CodigoLibro,CodigoUsuario,CantidadDias,Comentario,Estado")] Prestamo prestamo)
        {
            if (id != prestamo.CodigoPrestamo)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(prestamo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestamoExists(prestamo.CodigoPrestamo))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var empleados = await servicioEmpleado.ObtenerTodosAsync();
            var libros = await servicioLibro.ObtenerTodosAsync();
            var usuarios = await servicioEstudiantes.ObtenerTodosAsync();

            if (!empleados.Any())
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "EditPrestamo" });
            if (!libros.Any())
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "EditPrestamo" });
            if (!usuarios.Any())
                return RedirectToAction("Create", "Usuarios", new { RedirectedFrom = "EditPrestamo" });

            ViewData["Empleados"] = new SelectList(empleados, "CodigoEmpleado", "Nombre");
            ViewData["Libros"] = new SelectList(libros, "CodigoLibro", "Titulo");
            ViewData["Usuarios"] = new SelectList(usuarios, "CodigoUsuario", "Nombre");
            return View(prestamo);
        }

        // GET: Prestamos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await service.ObtenerPorIdAsync(id.Value);
            if (prestamo == null)
                return NotFound();

            return View(prestamo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool PrestamoExists(int id)
        {
            return context.Prestamos.Any(p => p.CodigoPrestamo == id);
        }
    }
}
