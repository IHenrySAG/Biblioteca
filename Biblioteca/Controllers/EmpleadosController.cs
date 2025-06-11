using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;

namespace Biblioteca.Controllers
{
    public class EmpleadosController(
        ContextoBiblioteca context,
        EmpleadoServicio service,
        ServicioBase<TandaLabor> servicioTandaLabor
    ) : Controller
    {
        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            var empleados = await service.ObtenerTodosAsync();
            return View(empleados);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);

            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        // GET: Empleados/Create
        public async Task<IActionResult> Create()
        {
            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (tandas.Count == 0)
                return RedirectToAction("Create", "TandasLabor", new { RedirectedFrom = "CreateEmpleado" });

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEmpleado,Nombre,Apellido,Cedula,CodigoTanda,PorcentajeComision,FechaIngreso,Estado")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(empleado);
                return RedirectToAction(nameof(Index));
            }

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (tandas.Count == 0)
                return RedirectToAction("Create", "TandasLabor", new { RedirectedFrom = "CreateEmpleado" });

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (tandas.Count == 0)
                return RedirectToAction("Create", "TandasLabor", new { RedirectedFrom = "EditEmpleado" });

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEmpleado,Nombre,Apellido,Cedula,CodigoTanda,PorcentajeComision,FechaIngreso,Estado")] Empleado empleado)
        {
            if (id != empleado.CodigoEmpleado)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(empleado);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.CodigoEmpleado))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (tandas.Count == 0)
                return RedirectToAction("Create", "TandasLabor", new { RedirectedFrom = "EditEmpleado" });

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return context.Empleados.Any(e => e.CodigoEmpleado == id);
        }
    }
}
