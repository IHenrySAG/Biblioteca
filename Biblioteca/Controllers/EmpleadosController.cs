using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Views.Empleados;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN))]
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
        [Authorization(nameof(ERoles.BIBLIOTECARIO))]
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var idUsuarioActual = HttpContext.Session.GetInt32("CodigoEmpleado");
            var rolUsuarioActual = HttpContext.Session.GetString("Rol");

            if (rolUsuarioActual!=nameof(ERoles.ADMIN) && (idUsuarioActual == null || idUsuarioActual != id))
            {
                // Solo el usuario actual puede ver sus propios detalles
                return RedirectToAction(nameof(HomeController.Error401), "Home");
            }

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

            var roles = await context.Roles
                .OrderByDescending(x=>x.NombreRol)
                .ToListAsync();

            ViewData["Roles"] = new SelectList(roles, "CodigoRol", "NombreRol");
            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEmpleado,Nombre,Apellido,Cedula,CodigoTanda,PorcentajeComision,FechaIngreso,NombreUsuario,Contrasenia,RepetirContrasenia,CodigoRol,Estado")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                if (empleado.Contrasenia != empleado.RepetirContrasenia)
                {
                    ModelState.AddModelError("RepetirContrasenia", "Las contraseñas no coinciden.");
                    return View(empleado);
                }



                await service.AgregarAsync(empleado);
                return RedirectToAction(nameof(Index));
            }

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            var roles = await context.Roles
                .OrderByDescending(x => x.NombreRol)
                .ToListAsync();

            ViewData["Roles"] = new SelectList(roles, "CodigoRol", "NombreRol");
            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");

            if (empleado.Contrasenia != empleado.RepetirContrasenia)
            {
                ModelState.AddModelError("RepetirContrasenia", "Las contraseñas no coinciden.");
            }

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

        // GET: Empleados/CambiarContrasenia/5
        public async Task<IActionResult> CambiarContrasenia(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            string rolEmpleadoActual = HttpContext.Session.GetString("Rol");

            ViewBag.NombreUsuario = empleado.NombreUsuario;
            ViewBag.IsAdmin = rolEmpleadoActual == nameof(ERoles.ADMIN);

            return View(new CambiarContrasenia
            {
                CodigoEmpleado = empleado.CodigoEmpleado,
                ContraseniaActual = string.Empty,
                NuevaContrasenia = string.Empty,
                ConfirmarContrasenia = string.Empty,
                MensajeError = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasenia(CambiarContrasenia model)
        {
            var empleado=await service.ObtenerPorIdAsync(model.CodigoEmpleado);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos correctamente.");
                return View(model);
            }

            if (empleado == null)
            {
                ModelState.AddModelError("", "Empleado no encontrado.");
                return View(model);
            }
            
            if (model.NuevaContrasenia != model.ConfirmarContrasenia)
            {
                ModelState.AddModelError("ConfirmarContrasenia", "Las contraseñas no coinciden.");
                return View(model);
            }

            if (model.NuevaContrasenia.Length < 8)
            {
                ModelState.AddModelError("NuevaContrasenia", "La nueva contraseña debe tener al menos 8 caracteres.");
                return View(model);
            }

            if (Encryption.GetMD5(model.ContraseniaActual) != empleado.Contrasenia)
            {
                ModelState.AddModelError("ContraseniaActual", "La contraseña actual es incorrecta.");
                return View(model);
            }

            if (Encryption.GetMD5(model.NuevaContrasenia) == empleado.Contrasenia)
            {
                ModelState.AddModelError("NuevaContrasenia", "La nueva contraseña no puede ser la misma que la actual.");
                return View(model);
            }

            empleado.Contrasenia = Encryption.GetMD5(model.NuevaContrasenia);

            try
            {
                await service.ActualizarAsync(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cambiar la contraseña: {ex.Message}");
                return View(model);
            }

            string rolEmpleado = HttpContext.Session.GetString("Rol");
            int codigoEmpleado = HttpContext.Session.GetInt32("CodigoEmpleado") ?? 0;

            return rolEmpleado == nameof(ERoles.ADMIN)
                ? RedirectToAction(nameof(Index))
                : RedirectToAction(nameof(Detalles), codigoEmpleado);
        }

        private bool EmpleadoExists(int id)
        {
            return context.Empleados.Any(e => e.CodigoEmpleado == id);
        }
    }
}
