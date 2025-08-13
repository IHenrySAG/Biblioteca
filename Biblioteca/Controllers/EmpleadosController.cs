using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Views.Empleados;
using Microsoft.IdentityModel.Tokens;
using Biblioteca.Model.ViewModel;

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
        public async Task<IActionResult> Index(string? filtro)
        {
            var empleados = await service.ObtenerTodosAsync(filtro);

            ViewBag.Filtro = filtro;

            return View(empleados);
        }

        // GET: Empleados/Detalles/5
        [Authorization(nameof(ERoles.BIBLIOTECARIO), nameof(ERoles.CATALOGADOR))]
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var idUsuarioActual = HttpContext.Session.GetInt32("CodigoEmpleado");
            var rolUsuarioActual = HttpContext.Session.GetString("Rol");

            if (rolUsuarioActual!=nameof(ERoles.ADMIN) && (idUsuarioActual == null || idUsuarioActual != id))
            {
                // Solo el usuario actual puede ver sus propios detalles
                return RedirectToAction(nameof(HomeController.Error403), "Home");
            }

            var empleado = await service.ObtenerPorIdAsync(id.Value);

            if (empleado == null)
                return NotFound();

            ViewBag.EditarDatos = rolUsuarioActual == nameof(ERoles.ADMIN);
            ViewBag.Rol = HttpContext.Session.GetString("Rol");

            return View(empleado);
        }

        // GET: Empleados/Crear
        public async Task<IActionResult> Crear()
        {
            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (!tandas.Any())
                ViewBag.ErrorTandas = "No hay tandas de horario registradas. Por favor, registre al menos una tanda antes de registrar un empleado.";

            var roles = await context.Roles
                .OrderByDescending(x=>x.NombreRol)
                .ToListAsync();

            ViewData["Roles"] = new SelectList(roles, "CodigoRol", "NombreRol");
            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View();
        }

        // POST: Empleados/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear( Empleado empleado)
        {
            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (!tandas.Any())
                ViewBag.ErrorTandas = "No hay tandas de horario registradas. Por favor, registre al menos una tanda antes de registrar un empleado.";

            var roles = await context.Roles
                .OrderByDescending(x => x.NombreRol)
                .ToListAsync();

            ViewData["Roles"] = new SelectList(roles, "CodigoRol", "NombreRol");
            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");

            if (ModelState.IsValid)
            {
                if (empleado.Contrasenia != empleado.RepetirContrasenia)
                {
                    ModelState.AddModelError("RepetirContrasenia", "Las contraseñas no coinciden.");
                    return View(empleado);
                }

                try
                {
                    await service.AgregarAsync(empleado);
                    return RedirectToAction(nameof(Index));
                }
                catch (InternalException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(empleado);
                }
            }

            return View(empleado);
        }

        // GET: Empleados/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (!tandas.Any())
                ViewBag.ErrorTandas = "No hay tandas de horario registradas. Por favor, registre al menos una tanda antes de editar un empleado.";

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            ViewBag.Roles = new SelectList(context.Roles.OrderByDescending(x => x.NombreRol), "CodigoRol", "NombreRol", empleado.CodigoRol);
            
            var empleadoVM=new EditarEmpleadoVM
            {
                CodigoEmpleado = empleado.CodigoEmpleado,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                Cedula = empleado.Cedula,
                CodigoTanda = empleado.CodigoTanda,
                FechaIngreso = empleado.FechaIngreso,
                NombreUsuario = empleado.NombreUsuario,
                CodigoRol = empleado.CodigoRol
            };
            return View(empleadoVM);
        }

        // POST: Empleados/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, EditarEmpleadoVM empleadoVM)
        {
            if (id != empleadoVM.CodigoEmpleado)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(empleadoVM);
                }
                catch(InternalException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewData["TandasLabor"] = new SelectList(await servicioTandaLabor.ObtenerTodosAsync(), "CodigoTanda", "NombreTanda");
                    return View(empleadoVM);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleadoVM.CodigoEmpleado))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var tandas = await servicioTandaLabor.ObtenerTodosAsync();

            if (!tandas.Any())
                ViewBag.ErrorTandas = "No hay tandas de horario registradas. Por favor, registre al menos una tanda antes de editar un empleado.";

            ViewData["TandasLabor"] = new SelectList(tandas, "CodigoTanda", "NombreTanda");
            return View(empleadoVM);
        }

        // GET: Empleados/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Empleados/CambiarContrasenia/5
        [Authorization(nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
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

        [Authorization(nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasenia(CambiarContrasenia model)
        {
            var empleado=await service.ObtenerPorIdAsync(model.CodigoEmpleado);

            if (empleado == null)
            {
                return NotFound();
            }

            string rolEmpleadoActual = HttpContext.Session.GetString("Rol");

            ViewBag.NombreUsuario = empleado.NombreUsuario;
            ViewBag.IsAdmin = rolEmpleadoActual == nameof(ERoles.ADMIN);

            if (!ModelState.IsValid || (model.ContraseniaActual ?? "").Trim().IsNullOrEmpty())
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos correctamente.");
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

            if (Encryption.GetMD5(model.ContraseniaActual ?? "") != empleado.Contrasenia)
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

            return RedirectToAction(nameof(Detalles), new { id = codigoEmpleado });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContraseniaAdmin(CambiarContrasenia model)
        {
            var empleado = await service.ObtenerPorIdAsync(model.CodigoEmpleado);

            if (empleado == null)
            {
                ModelState.AddModelError("", "Empleado no encontrado.");
                return View(nameof(CambiarContrasenia), model);
            }

            string rolEmpleadoActual = HttpContext.Session.GetString("Rol");

            ViewBag.NombreUsuario = empleado.NombreUsuario;
            ViewBag.IsAdmin = rolEmpleadoActual == nameof(ERoles.ADMIN);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Por favor, complete todos los campos correctamente.");
                return View(nameof(CambiarContrasenia), model);
            }

            if (model.NuevaContrasenia != model.ConfirmarContrasenia)
            {
                ModelState.AddModelError("ConfirmarContrasenia", "Las contraseñas no coinciden.");
                return View(nameof(CambiarContrasenia), model);
            }

            if (model.NuevaContrasenia.Length < 8)
            {
                ModelState.AddModelError("NuevaContrasenia", "La nueva contraseña debe tener al menos 8 caracteres.");
                return View(nameof(CambiarContrasenia), model);
            }

            if (Encryption.GetMD5(model.NuevaContrasenia) == empleado.Contrasenia)
            {
                ModelState.AddModelError("NuevaContrasenia", "La nueva contraseña no puede ser la misma que la actual.");
                return View(nameof(CambiarContrasenia), model);
            }

            empleado.Contrasenia = Encryption.GetMD5(model.NuevaContrasenia);

            try
            {
                await service.ActualizarAsync(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cambiar la contraseña: {ex.Message}");
                return View(nameof(CambiarContrasenia), model);
            }

            string rolEmpleado = HttpContext.Session.GetString("Rol");
            int codigoEmpleado = HttpContext.Session.GetInt32("CodigoEmpleado") ?? 0;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Empleados/{id}/Prestamos")]
        public async Task<IActionResult> PrestamosPorEmpleado(int? id, string? filtro)
        {
            if (id == null)
                return NotFound();

            var empleado = await service.ObtenerPorIdAsync(id.Value);
            if (empleado == null)
                return NotFound();

            var prestamos = empleado.Prestamos?
                .Where(p => !(p.Eliminado ?? false))
                .Where(string.IsNullOrEmpty(filtro) ?
                l => true :
                l => l.CodigoPrestamo.ToString().Contains(filtro) ||
                      l.Libro.Titulo.Contains(filtro) ||
                      l.Estudiante.Nombre.Contains(filtro) ||
                      l.Estudiante.Apellido.Contains(filtro) ||
                      l.FechaPrestamo.ToString().Contains(filtro) ||
                      (l.FechaDevolucion.ToString() ?? "").Contains(filtro) ||
                      l.FechaDevolucionEsperada.ToString().Contains(filtro) ||
                      l.MontoDia.ToString().Contains(filtro))
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

            ViewBag.Title = "Prestamos del empleado " + empleado.Nombre;
            ViewBag.CodigoEmpleado = empleado.CodigoEmpleado;
            ViewBag.Filtro = filtro;

            return View(prestamos);
        }

        private bool EmpleadoExists(int id)
        {
            return context.Empleados.Any(e => e.CodigoEmpleado == id);
        }
    }
}
