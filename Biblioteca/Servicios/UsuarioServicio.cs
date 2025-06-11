using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class UsuarioServicio(ContextoBiblioteca context) : ServicioBase<Usuario>(context)
{
    public override async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await context.Usuarios
            .Where(x => !(x.Eliminado ?? false))
            .Include(u => u.TipoPersona)
            .Include(u => u.Prestamos)
            .ToListAsync();
    }

    public override async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        var usuario = await context.Usuarios
            .Where(x => !(x.Eliminado ?? false))
            .Include(u => u.TipoPersona)
            .Include(u => u.Prestamos)
            .FirstOrDefaultAsync(u => u.CodigoUsuario == id);

        // Cargar detalles adicionales de cada préstamo (Libro y Empleado)
        foreach (var prestamo in usuario?.Prestamos ?? Enumerable.Empty<Prestamo>())
        {
            await context.Entry(prestamo).Reference(p => p.Libro).LoadAsync();
            await context.Entry(prestamo).Reference(p => p.Empleado).LoadAsync();
        }

        return usuario;
    }
}
