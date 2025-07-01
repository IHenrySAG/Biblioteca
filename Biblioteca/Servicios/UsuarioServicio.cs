using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class UsuarioServicio(ContextoBiblioteca context) : ServicioBase<Estudiante>(context)
{
    public override async Task<List<Estudiante>> ObtenerTodosAsync()
    {
        return await context.Estudiantes
            .Where(x => !(x.Eliminado ?? false))
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
}
