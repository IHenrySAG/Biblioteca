using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class IdiomaServicio(ContextoBiblioteca context) : ServicioBase<Idioma>(context)
{
    public override async Task<IEnumerable<Idioma>> ObtenerTodosAsync(string filtro=null)
    {
        return await context.Idiomas
            .AsNoTrackingWithIdentityResolution()
            .Where(x => !(x.Eliminado ?? false))
            .Include(i => i.Libros)
            .Include(i => i.Autores)
            .Where(string.IsNullOrEmpty(filtro) ?
                e => true :
                e => e.NombreIdioma.Contains(filtro))
            .ToListAsync();
    }

    public override async Task<Idioma?> ObtenerPorIdAsync(int id)
    {
        var idioma = await context.Idiomas
            .Where(x => !(x.Eliminado ?? false))
            .Include(i => i.Libros)
            .Include(i => i.Autores)
            .FirstOrDefaultAsync(i => i.CodigoIdioma == id);

        // Cargar detalles adicionales si es necesario (por ejemplo, Editora de cada libro)
        foreach (var libro in idioma?.Libros ?? Enumerable.Empty<Libro>())
        {
            await context.Entry(libro).Reference(l => l.Editora).LoadAsync();
        }

        return idioma;
    }
}
