using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class EmpleadoServicio(ContextoBiblioteca context) : ServicioBase<Empleado>(context)
{
    public override async Task<List<Empleado>> ObtenerTodosAsync()
    {
        return await context.Empleados
            .Where(x => !(x.Eliminado ?? false))
            .Include(e => e.TandaLabor)
            .Include(e => e.Prestamos)
            .ToListAsync();
    }

    public override async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        var empleado = await context.Empleados
            .Where(x => !(x.Eliminado ?? false))
            .Include(e => e.TandaLabor)
            .Include(e => e.Prestamos)
            .FirstOrDefaultAsync(e => e.CodigoEmpleado == id);

        // Cargar los libros y usuarios relacionados a través de los préstamos
        foreach (var prestamo in empleado?.Prestamos ?? Enumerable.Empty<Prestamo>())
        {
            await context.Entry(prestamo).Reference(p => p.Libro).LoadAsync();
            await context.Entry(prestamo).Reference(p => p.Usuario).LoadAsync();
        }

        return empleado;
    }
}
