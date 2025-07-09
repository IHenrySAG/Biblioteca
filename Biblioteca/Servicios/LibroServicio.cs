using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class LibroServicio(ContextoBiblioteca context) : ServicioBase<Libro>(context)
{
    public override async Task<IEnumerable<Libro>> ObtenerTodosAsync()
    {
        return await context.Libros
            .Where(x=>!(x.Eliminado??false))
            .Include(l => l.Idioma)
            .Include(l => l.Editora)
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerConFiltroAsync(string filtro)
    {
        return await context.Libros
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Idioma)
            .Include(l => l.Editora)
            .Where(string.IsNullOrEmpty(filtro) ?
                l => true :
                l => l.Titulo.Contains(filtro) ||
                      l.SignaturaTopografica.Contains(filtro) ||
                      l.Isbn.Contains(filtro) ||
                      l.Editora.NombreEditora.Contains(filtro) ||
                      l.Idioma.NombreIdioma.Contains(filtro))
            .ToListAsync();
    }

    public override async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        var libro = await context.Libros
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Idioma)
            .Include(l => l.Editora)
            .Include(l => l.LibrosBibliografias)
            .Include(l => l.LibrosAutores)
            .Where(l => l.CodigoLibro == id)
            .FirstOrDefaultAsync();

        foreach (var libroBibliografia in libro?.LibrosBibliografias ?? Enumerable.Empty<LibroBibliografia>())
        {
            await context.Entry(libroBibliografia).Reference(la => la.TipoBibliografia).LoadAsync();
        }

        foreach (var libroAutor in libro?.LibrosAutores ?? Enumerable.Empty<LibroAutor>())
        {
            await context.Entry(libroAutor).Reference(la => la.Autor).LoadAsync();
        }

        return libro;
    }

}
