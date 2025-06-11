using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class AutorServicio(ContextoBiblioteca context) : ServicioBase<Autor>(context)
{
    public override async Task<List<Autor>> ObtenerTodosAsync()
    {
        return await context.Autores
            .Where(x => !(x.Eliminado ?? false))
            .Include(a => a.Idioma)
            .Include(a => a.LibrosAutores)
            .ToListAsync();
    }

    public override async Task<Autor?> ObtenerPorIdAsync(int id)
    {
        var autor = await context.Autores
            .Where(x => !(x.Eliminado ?? false))
            .Include(a => a.Idioma)
            .Include(a => a.LibrosAutores)
            .FirstOrDefaultAsync(a => a.CodigoAutor == id);

        // Cargar los libros relacionados a través de la relación muchos a muchos
        foreach (var libroAutor in autor?.LibrosAutores ?? Enumerable.Empty<LibroAutor>())
        {
            await context.Entry(libroAutor).Reference(la => la.Libro).LoadAsync();
        }

        return autor;
    }
}
