using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class PrestamoServicio(ContextoBiblioteca context) : ServicioBase<Prestamo>(context)
{
    public override async Task<List<Prestamo>> ObtenerTodosAsync()
    {
        return await context.Prestamos
            //.Where(x => !(x.Eliminado ?? false))
            //.Include(p => p.Empleado)
            //.Include(p => p.Libro)
            //.Include(p => p.Usuario)
            .ToListAsync();
    }

    public override async Task<Prestamo?> ObtenerPorIdAsync(int id)
    {
        var prestamo = await context.Prestamos
            .Where(x => !(x.Eliminado ?? false))
            .Include(p => p.Empleado)
            .Include(p => p.Libro)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.CodigoPrestamo == id);

        // Cargar detalles adicionales si es necesario (por ejemplo, Editora e Idioma del libro)
        if (prestamo?.Libro != null)
        {
            await context.Entry(prestamo.Libro).Reference(l => l.Editora).LoadAsync();
            await context.Entry(prestamo.Libro).Reference(l => l.Idioma).LoadAsync();
        }

        return prestamo;
    }
}
