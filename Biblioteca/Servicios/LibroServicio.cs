using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Model;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Servicios;
public class LibroServicio(ContextoBiblioteca context) : ServicioBase<Libro>(context)
{
    public override async Task<IEnumerable<Libro>> ObtenerTodosAsync(string filtro=null)
    {
        return await ConsultarListaConFiltro(filtro)
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorIdIdiomaAsync(int id, string? filtro)
    {
        return await ConsultarListaConFiltro(filtro)
            .Where(l => l.CodigoIdioma == id)
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorIdEditoraAsync(int id, string? filtro)
    {
        return await ConsultarListaConFiltro(filtro)
            .Where(l => l.CodigoEditora == id)
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorIdAutorAsync(int id, string? filtro)
    {
        return await ConsultarListaConFiltro(filtro)
            .Where(l => l.LibrosAutores.Any(x=>x.CodigoAutor==id))
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorIdBibliografiaAsync(int id, string? filtro)
    {
        return await ConsultarListaConFiltro(filtro)
            .Where(l => l.CodigoBibliografia == id)
            .ToListAsync();
    }

    public override async Task<Libro?> ObtenerPorIdAsync(int id)
    {
        var libro = await context.Libros
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Idioma)
            .Include(l => l.Editora)
            .Include(l => l.TipoBibliografia)
            .Include(l => l.LibrosAutores)
            .ThenInclude(la=>la.Autor)
            .Where(l => l.CodigoLibro == id)
            .FirstOrDefaultAsync();

        //foreach (var libroBibliografia in libro?.LibrosBibliografias ?? Enumerable.Empty<LibroBibliografia>())
        //{
        //    await context.Entry(libroBibliografia).Reference(la => la.TipoBibliografia).LoadAsync();
        //}

        foreach (var libroAutor in libro?.LibrosAutores ?? Enumerable.Empty<LibroAutor>())
        {
            await context.Entry(libroAutor).Reference(la => la.Autor).LoadAsync();
        }

        return libro;
    }

    public override async Task<Libro> AgregarAsync(Libro libro)
    {
        await context.Database.BeginTransactionAsync();
        await base.AgregarAsync(libro);

        await context.Database.CommitTransactionAsync();
        return libro;
    }

    public override async Task<Libro> ActualizarAsync(Libro entidad)
    {
        var libro = await ObtenerPorIdAsync(entidad.CodigoLibro);

        libro.Titulo = entidad.Titulo;
        libro.SignaturaTopografica = entidad.SignaturaTopografica;
        libro.Isbn = entidad.Isbn;
        libro.CodigoEditora = entidad.CodigoEditora;
        libro.AnioPublicacion = entidad.AnioPublicacion;
        libro.Ciencia = entidad.Ciencia;
        libro.CodigoIdioma = entidad.CodigoIdioma;
        libro.CodigoBibliografia = entidad.CodigoBibliografia;
        libro.Inventario = entidad.Inventario;

        //var bibliografiasAEliminar = libro.LibrosBibliografias.Select(x => x.TipoBibliografia);

        //context.TiposBibliografias.RemoveRange(bibliografiasAEliminar);
        //context.LibrosBibliografias.RemoveRange(libro.LibrosBibliografias);
        context.LibrosAutores.RemoveRange(libro.LibrosAutores);

        //libro.LibrosBibliografias = entidad.LibrosBibliografias;
        libro.LibrosAutores = entidad.LibrosAutores;

        await context.SaveChangesAsync();

        return libro;
    }

    private IQueryable<Libro> ConsultarListaConFiltro(string filtro)
    {
        return context.Libros
            .AsNoTrackingWithIdentityResolution()
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Idioma)
            .Include(l => l.Editora)
            .Include(l => l.TipoBibliografia)
            .Include(l => l.LibrosAutores)
            .ThenInclude(la => la.Autor)
            .Where(string.IsNullOrEmpty(filtro) ?
                l => true :
                l => l.Titulo.Contains(filtro) ||
                      l.SignaturaTopografica.Contains(filtro) ||
                      l.Isbn.Contains(filtro) ||
                      l.TipoBibliografia.NombreBibliografia.Contains(filtro) ||
                      l.Editora.NombreEditora.Contains(filtro) ||
                      l.LibrosAutores.Any(la=>la.Autor.NombreAutor.Contains(filtro)) ||
                      l.Idioma.NombreIdioma.Contains(filtro));
    }
}
