using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class TipoBibliografiaServicio(ContextoBiblioteca context) : ServicioBase<TipoBibliografia>(context)
{
    public override async Task<IEnumerable<TipoBibliografia>> ObtenerTodosAsync()
    {
        return await context.TiposBibliografias
            .Where(x => !(x.Eliminado ?? false))
            .Include(tb => tb.LibrosBibliografias)
            .ToListAsync();
    }

    public override async Task<TipoBibliografia?> ObtenerPorIdAsync(int id)
    {
        var tipo = await context.TiposBibliografias
            .Where(x => !(x.Eliminado ?? false))
            .Include(tb => tb.LibrosBibliografias)
            .FirstOrDefaultAsync(tb => tb.CodigoBibliografia == id);

        // Cargar los libros relacionados a través de la relación muchos a muchos
        foreach (var libroBibliografia in tipo?.LibrosBibliografias ?? Enumerable.Empty<LibroBibliografia>())
        {
            await context.Entry(libroBibliografia).Reference(lb => lb.Libro).LoadAsync();
        }

        return tipo;
    }
}
