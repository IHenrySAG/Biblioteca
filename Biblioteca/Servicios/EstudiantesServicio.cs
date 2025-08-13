using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class EstudiantesServicio(ContextoBiblioteca context) : ServicioBase<Estudiante>(context)
{
    public override async Task<IEnumerable<Estudiante>> ObtenerTodosAsync(string? filtro)
    {
        return await context.Estudiantes
            .AsNoTrackingWithIdentityResolution()
            .Where(x => !(x.Eliminado ?? false))
            .Where(string.IsNullOrEmpty(filtro) ?
                e => true :
                e => e.Nombre.Contains(filtro) || e.Apellido.Contains(filtro) || e.Cedula.Contains(filtro) || e.NumeroCarnet.Contains(filtro) || (e.TipoPersona != null && e.TipoPersona.NombreTipo.Contains(filtro)))
            .Include(u => u.TipoPersona)
            .Include(u => u.Prestamos)
            .ToListAsync();
    }

    public override async Task<Estudiante?> ObtenerPorIdAsync(int id)
    {
        var usuario = await context.Estudiantes
            .Where(x => !(x.Eliminado ?? false))
            .Include(u => u.TipoPersona)
            .Include(u => u.Prestamos)
            .FirstOrDefaultAsync(u => u.CodigoEstudiante == id);

        // Cargar detalles adicionales de cada préstamo (Libro y Empleado)
        foreach (var prestamo in usuario?.Prestamos ?? Enumerable.Empty<Prestamo>())
        {
            await context.Entry(prestamo).Reference(p => p.Libro).LoadAsync();
            await context.Entry(prestamo).Reference(p => p.Empleado).LoadAsync();
        }

        return usuario;
    }

    public async Task<Estudiante?> ObtenerPorCarnetAsync(string carnet)
    {
        return await context.Estudiantes
            .AsNoTrackingWithIdentityResolution()
            .Include(u => u.Prestamos)
            .Where(x => x.NumeroCarnet == carnet.Trim())
            .FirstOrDefaultAsync()
        ?? null;
    }

    public async Task<Estudiante?> ObtenerPorCedulaAsync(string cedula)
    {
        return await context.Estudiantes
            .AsNoTrackingWithIdentityResolution()
            .Include(u => u.Prestamos)
            .Where(x => x.Cedula == cedula.Trim())
            .FirstOrDefaultAsync()
        ?? null;
    }
}
