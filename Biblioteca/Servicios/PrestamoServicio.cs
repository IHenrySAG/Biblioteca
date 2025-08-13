using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class PrestamoServicio(ContextoBiblioteca context) : ServicioBase<Prestamo>(context)
{
    public override async Task<IEnumerable<Prestamo>> ObtenerTodosAsync(string filtro=null)
    {
        return await ConsultarTodosConFiltro(filtro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerVencidosAsync(string? filtro)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.FechaDevolucion == null)
            .Where(x => DateOnly.FromDateTime(DateTime.Now) > x.FechaDevolucionEsperada)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerPendientesAsync(string? filtro)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.FechaDevolucion == null)
            .Where(x => DateOnly.FromDateTime(DateTime.Now) <= x.FechaDevolucionEsperada)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerDevueltosAsync(string? filtro)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.FechaDevolucion != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerPorIdEmpleadoAsync(int idEmpleado, string? filtro=null)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.CodigoEmpleado == idEmpleado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerPorIdLibroAsync(int idLibro, string? filtro = null)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.CodigoLibro == idLibro)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> ObtenerPorIdEstudianteAsync(int idEstudiante, string? filtro = null)
    {
        return await ConsultarTodosConFiltro(filtro)
            .Where(x => x.CodigoEstudiante == idEstudiante)
            .ToListAsync();
    }

    public override async Task<Prestamo?> ObtenerPorIdAsync(int id)
    {
        var prestamo = await context.Prestamos
            .Where(x => !(x.Eliminado ?? false))
            .Include(p => p.Empleado)
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .FirstOrDefaultAsync(p => p.CodigoPrestamo == id);

        // Cargar detalles adicionales si es necesario (por ejemplo, Editora e Idioma del libro)
        if (prestamo?.Libro != null)
        {
            await context.Entry(prestamo.Libro).Reference(l => l.Editora).LoadAsync();
            await context.Entry(prestamo.Libro).Reference(l => l.Idioma).LoadAsync();
        }

        return prestamo;
    }

    private IQueryable<Prestamo> ConsultarTodosConFiltro(string? filtro=null)
    {
        return context.Prestamos
        .AsNoTrackingWithIdentityResolution()
        .Where(x => !(x.Eliminado ?? false))
        .Include(p => p.Libro)
        .Include(p => p.Estudiante)
        .Include(p => p.Empleado)
        .OrderByDescending(x => x.FechaDevolucionEsperada)
        .Where(string.IsNullOrEmpty(filtro) ?
                l => true :
                l => l.CodigoPrestamo.ToString().Contains(filtro) ||
                      l.Libro.Titulo.Contains(filtro) ||
                      l.Estudiante.Nombre.Contains(filtro) ||
                      l.Estudiante.Apellido.Contains(filtro) ||
                      l.FechaPrestamo.ToString().Contains(filtro) ||
                      (l.FechaDevolucion.ToString() ?? "").Contains(filtro) ||
                      l.FechaDevolucionEsperada.ToString().Contains(filtro) ||
                      l.MontoDia.ToString().Contains(filtro));
    }
}
