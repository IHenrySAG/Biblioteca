using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Biblioteca.Model.ViewModel;
using Biblioteca.Model.DTOs;
using System.Text;

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
        [HttpGet]
        public async Task<IActionResult> Index(string? filtro)
        {
            var prestamos = await service.ObtenerTodosAsync(filtro);

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

            ViewBag.Filtro = filtro;
            ViewBag.Title = "Lista de prestamos";

            ViewBag.ListaEstados = new List<SelectListItem>
            {
                new SelectListItem("Todos", "", true),
                new SelectListItem("Vencidos", CEstadoPrestamo.VENCIDO),
                new SelectListItem("Pendientes", CEstadoPrestamo.PENDIENTE),
                new SelectListItem("Devueltos", CEstadoPrestamo.DEVUELTO)
            };

            return View(prestamosVM);
        }

        [HttpGet("Prestamos/Estado/{estadoPrestamo}")]
        public async Task<IActionResult> Index([FromRoute] string estadoPrestamo, string? filtro)
        {
            if (string.IsNullOrEmpty(estadoPrestamo))
                return RedirectToAction(nameof(Index));

            IEnumerable<Prestamo> prestamos;

            switch (estadoPrestamo)
            {
                case CEstadoPrestamo.VENCIDO:
                    prestamos = await service.ObtenerVencidosAsync(filtro);
                    ViewBag.Title = "Lista de Préstamos Vencidos";
                    break;
                case CEstadoPrestamo.PENDIENTE:
                    prestamos = await service.ObtenerPendientesAsync(filtro);
                    ViewBag.Title = "Lista de Préstamos Pendientes";
                    break;
                case CEstadoPrestamo.DEVUELTO:
                    prestamos = await service.ObtenerDevueltosAsync(filtro);
                    ViewBag.Title = "Lista de Préstamos Devueltos";
                    break;
                default:
                    return RedirectToAction(nameof(Index));
            }

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

            ViewBag.EstadoPrestamo = estadoPrestamo;
            ViewBag.Filtro = filtro;

            ViewBag.ListaEstados = new List<SelectListItem>
            {
                new SelectListItem("Todos", ""),
                new SelectListItem("Vencidos", CEstadoPrestamo.VENCIDO, estadoPrestamo==CEstadoPrestamo.VENCIDO),
                new SelectListItem("Pendientes", CEstadoPrestamo.PENDIENTE, estadoPrestamo==CEstadoPrestamo.PENDIENTE),
                new SelectListItem("Devueltos", CEstadoPrestamo.DEVUELTO, estadoPrestamo==CEstadoPrestamo.DEVUELTO)
            };

            return View(nameof(Index), prestamosVM);
        }

        // GET: Prestamos/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var prestamo = await service.ObtenerPorIdAsync(id.Value);

            if (prestamo == null)  
                return NotFound();

            return View(prestamo);
        }

        // GET: Prestamos/Crear
        public async Task<IActionResult> Crear()
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

        // POST: Prestamos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Prestamo prestamo)
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

        // GET: Prestamos/DevolverLibro/5
        public async Task<IActionResult> DevolverLibro(int id)
        {
            var prestamo = await service.ObtenerPorIdAsync(id);
            if (prestamo == null)
                return NotFound();

            if (prestamo.FechaDevolucion.HasValue)
            {
                ViewBag.ErrorLibro = $"El libro \"{prestamo.Libro.Titulo}\", correspondiente a este prestamo, ya ha sido devuelto.";
                return View(prestamo);
            }

            int cantidadDias = (DateTime.Now - prestamo.FechaPrestamo.Value.ToDateTime(TimeOnly.MinValue)).Days;
            decimal totalDias = (prestamo.MontoDia ?? 0) * cantidadDias;

            int cantidadDiasRetraso = prestamo.FechaDevolucionEsperada < DateOnly.FromDateTime(DateTime.Now.Date)
                ? (DateTime.Now - prestamo.FechaDevolucionEsperada.Value.ToDateTime(TimeOnly.MinValue)).Days
                : 0;
            decimal totalDiasRetraso = (prestamo.MontoDiaRetraso ?? 0) * cantidadDiasRetraso;

            ViewBag.CantidadDias = cantidadDias;
            ViewBag.CantidadDiasRetraso = cantidadDiasRetraso;
            ViewBag.TotalDias = totalDias;
            ViewBag.TotalDiasRetraso = totalDiasRetraso;
            ViewBag.Total = totalDias + totalDiasRetraso;

            return View(prestamo);
        }

        // GET: Prestamos/DevolverLibro/5
        [HttpPost]
        public async Task<IActionResult> DevolverLibro(int id, string comentario)
        {
            var prestamo = await service.ObtenerPorIdAsync(id);
            if (prestamo == null)
                return NotFound();

            if (prestamo.FechaDevolucion.HasValue)
            {
                ViewBag.ErrorLibro = $"El libro \"{prestamo.Libro.Titulo}\", correspondiente a este prestamo, ya ha sido devuelto.";
                return View(prestamo);
            }

            var libro = await servicioLibro.ObtenerPorIdAsync(prestamo.CodigoLibro);
            if(libro is null)
            {
                return NotFound();
            }

            int cantidadDias = (DateTime.Now - prestamo.FechaPrestamo.Value.ToDateTime(TimeOnly.MinValue)).Days;
            decimal totalDias = (prestamo.MontoDia ?? 0) * cantidadDias;

            int cantidadDiasRetraso = prestamo.FechaDevolucionEsperada < DateOnly.FromDateTime(DateTime.Now.Date)
                ? (DateTime.Now - prestamo.FechaDevolucionEsperada.Value.ToDateTime(TimeOnly.MinValue)).Days
                : 0;
            decimal totalDiasRetraso = (prestamo.MontoDiaRetraso ?? 0) * cantidadDiasRetraso;

            decimal total = totalDias + totalDiasRetraso;

            prestamo.FechaDevolucion = DateOnly.FromDateTime(DateTime.Now.Date);
            prestamo.MontoTotal = total;
            prestamo.Comentario = comentario;

            libro.Inventario++;

            await service.ActualizarAsync(prestamo);
            await servicioLibro.ActualizarAsync(libro);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportarPrestamosCsv(string estado)
        {
            var prestamos = estado switch
            {
                CEstadoPrestamo.PENDIENTE => await service.ObtenerPendientesAsync(null),
                CEstadoPrestamo.VENCIDO => await service.ObtenerVencidosAsync(null),
                CEstadoPrestamo.DEVUELTO => await service.ObtenerDevueltosAsync(null),
                _ => await service.ObtenerTodosAsync()
            };

            var fileInfo = GenerarCsvPrestamos(prestamos, estado);
            
            return File(System.Text.Encoding.UTF8.GetBytes(fileInfo.Item1.ToString()), "text/csv", fileInfo.Item2);
        }

        public async Task<IActionResult> ExportarPrestamosPorEmpleadoCsv(int idEmpleado)
        {
            var empleado = await servicioEmpleado.ObtenerPorIdAsync(idEmpleado);
            if (empleado is null)
                return NotFound();

            var prestamos = await service.ObtenerPorIdEmpleadoAsync(idEmpleado);

            var fileInfo = GenerarCsvPrestamos(prestamos, empleado.Nombre+"_"+empleado.Apellido);

            return File(System.Text.Encoding.UTF8.GetBytes(fileInfo.Item1.ToString()), "text/csv", fileInfo.Item2);
        }

        public async Task<IActionResult> ExportarPrestamosPorLibroCsv(int idLibro)
        {
            var libro = await servicioLibro.ObtenerPorIdAsync(idLibro);
            if (libro is null)
                return NotFound();

            var prestamos = await service.ObtenerPorIdLibroAsync(idLibro);

            var fileInfo = GenerarCsvPrestamos(prestamos, libro.Titulo);

            return File(System.Text.Encoding.UTF8.GetBytes(fileInfo.Item1.ToString()), "text/csv", fileInfo.Item2);
        }

        public async Task<IActionResult> ExportarPrestamosPorEstudianteCsv(int idEstudiante)
        {
            var empleado = await servicioEstudiantes.ObtenerPorIdAsync(idEstudiante);
            if (empleado is null)
                return NotFound();

            var prestamos = await service.ObtenerPorIdEstudianteAsync(idEstudiante);

            var fileInfo = GenerarCsvPrestamos(prestamos, empleado.Nombre + "_" + empleado.Apellido);

            return File(System.Text.Encoding.UTF8.GetBytes(fileInfo.Item1.ToString()), "text/csv", fileInfo.Item2);
        }

        public async Task<IActionResult> ExportarPrestamosXml(string estado)
        {
            var prestamos = estado switch
            {
                CEstadoPrestamo.PENDIENTE => await service.ObtenerPendientesAsync(null),
                CEstadoPrestamo.VENCIDO => await service.ObtenerVencidosAsync(null),
                CEstadoPrestamo.DEVUELTO => await service.ObtenerDevueltosAsync(null),
                _ => await service.ObtenerTodosAsync()
            };

            if (!prestamos.Any())
            {
                return NotFound();
            }

            var dtos = prestamos.Select(prestamo => new PrestamoExportDto
            {
                CodigoPrestamo=prestamo.CodigoPrestamo,
                Estudiante=prestamo.Estudiante.NumeroCarnet +" - "+ prestamo.Estudiante.Nombre+" "+prestamo.Estudiante.Apellido,
                FechaDevolucion = prestamo.FechaDevolucion?.ToDateTime(new()).ToString("dd/MM/yyyy"),
                FechaDevolucionEsperada = prestamo.FechaDevolucionEsperada?.ToDateTime(new()).ToString("dd/MM/yyyy"),
                FechaPrestamo = prestamo.FechaPrestamo?.ToDateTime(new()).ToString("dd/MM/yyyy"),
                Libro = prestamo.Libro.Titulo,
                Empleado=prestamo.Empleado.Nombre + " " + prestamo.Empleado.Apellido,
                MontoDia = prestamo.MontoDia,
                MontoDiaRetraso = prestamo.MontoDiaRetraso,
                MontoTotal = prestamo.MontoTotal
            }).ToList();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PrestamoExportDto>));

            using var ms = new System.IO.MemoryStream();
            serializer.Serialize(ms, dtos);
            ms.Position = 0;
            var titulo = $"Lista_Prestamos{(estado is not null ? "_" + estado : "")}_{DateTime.Now.Date:yyyy_MM_dd}";

            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                titulo = titulo.Replace(c, '-');
            }
            var fileName = $"{titulo}.xml";
            return File(ms.ToArray(), "text/plain", fileName);
        }

        private bool PrestamoExists(int id)
        {
            return context.Prestamos.Any(p => p.CodigoPrestamo == id);
        }

        private (StringBuilder, string) GenerarCsvPrestamos(IEnumerable<Prestamo> prestamos, string nombreArchivo = null)
        {
            var prestamosVM = prestamos.Select(prestamo => new PrestamoVM
            {
                CodigoPrestamo = prestamo.CodigoPrestamo,
                Estudiante = prestamo.Estudiante.NumeroCarnet + " - " + prestamo.Estudiante.Nombre + " " + prestamo.Estudiante.Apellido,
                FechaDevolucion = prestamo.FechaDevolucion,
                FechaDevolucionEsperada = prestamo.FechaDevolucionEsperada,
                FechaPrestamo = prestamo.FechaPrestamo,
                Libro = prestamo.Libro.Titulo,
                Empleado = prestamo.Empleado.Nombre + " " + prestamo.Empleado.Apellido,
                MontoDia = prestamo.MontoDia,
                MontoDiaRetraso = prestamo.MontoDiaRetraso,
                MontoTotal = prestamo.MontoTotal
            }).ToList();

            var sb = new System.Text.StringBuilder();
            var titulosPropiedades = typeof(PrestamoVM).GetProperties()
                .Select(x => x.Name);

            sb.AppendLine(string.Join(";", titulosPropiedades));

            foreach (var prestamo in prestamosVM)
            {
                List<string> valores = [];
                foreach (var titulo in titulosPropiedades)
                {
                    valores.Add(prestamo.GetType().GetProperty(titulo)?.GetValue(prestamo)?.ToString().Replace(";", ",") ?? "");
                }
                sb.AppendLine(string.Join(";", valores));
            }
            var fileName = $"Lista_Prestamos{(nombreArchivo is not null ? "_" + nombreArchivo : "")}_{DateTime.Now.Date:yyyy_MM_dd}";
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            fileName += ".csv";

            return (sb, fileName);
        }
    }
}
