using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;

namespace Biblioteca.Controllers
{
    public class PrestamosController(
        ContextoBiblioteca context,
        PrestamoServicio service,
        ServicioBase<Empleado> servicioEmpleado,
        ServicioBase<Libro> servicioLibro,
        ServicioBase<Usuario> servicioUsuario
    ) : Controller
    {
        // GET: Prestamos
        public async Task<IActionResult> Index()
        {
            var prestamos = await service.ObtenerTodosAsync();
            return View(prestamos);
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
            var empleados = await servicioEmpleado.ObtenerTodosAsync();
            var libros = await servicioLibro.ObtenerTodosAsync();
            var usuarios = await servicioUsuario.ObtenerTodosAsync();

            if (empleados.Count == 0)
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "CreatePrestamo" });
            if (libros.Count == 0)
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "CreatePrestamo" });
            if (usuarios.Count == 0)
                return RedirectToAction("Create", "Usuarios", new { RedirectedFrom = "CreatePrestamo" });

            ViewData["Empleados"] = new SelectList(empleados, "CodigoEmpleado", "Nombre");
            ViewData["Libros"] = new SelectList(libros, "CodigoLibro", "Titulo");
            ViewData["Usuarios"] = new SelectList(usuarios, "CodigoUsuario", "Nombre");
            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoPrestamo,CodigoEmpleado,CodigoLibro,CodigoUsuario,CantidadDias,Comentario,Estado")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(prestamo);
                return RedirectToAction(nameof(Index));
            }

            var empleados = await servicioEmpleado.ObtenerTodosAsync();
            var libros = await servicioLibro.ObtenerTodosAsync();
            var usuarios = await servicioUsuario.ObtenerTodosAsync();

            if (empleados.Count == 0)
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "CreatePrestamo" });
            if (libros.Count == 0)
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "CreatePrestamo" });
            if (usuarios.Count == 0)
                return RedirectToAction("Create", "Usuarios", new { RedirectedFrom = "CreatePrestamo" });

            ViewData["Empleados"] = new SelectList(empleados, "CodigoEmpleado", "Nombre");
            ViewData["Libros"] = new SelectList(libros, "CodigoLibro", "Titulo");
            ViewData["Usuarios"] = new SelectList(usuarios, "CodigoUsuario", "Nombre");
            return View(prestamo);
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
            var usuarios = await servicioUsuario.ObtenerTodosAsync();

            if (empleados.Count == 0)
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "EditPrestamo" });
            if (libros.Count == 0)
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "EditPrestamo" });
            if (usuarios.Count == 0)
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
            var usuarios = await servicioUsuario.ObtenerTodosAsync();

            if (empleados.Count == 0)
                return RedirectToAction("Create", "Empleados", new { RedirectedFrom = "EditPrestamo" });
            if (libros.Count == 0)
                return RedirectToAction("Create", "Libros", new { RedirectedFrom = "EditPrestamo" });
            if (usuarios.Count == 0)
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
