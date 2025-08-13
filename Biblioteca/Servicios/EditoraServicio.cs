using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class EditoraServicio(ContextoBiblioteca context) : ServicioBase<Editora>(context)
{
    public override async Task<IEnumerable<Editora>> ObtenerTodosAsync(string? filtro=null)
    {
        return await context.Editoras
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Libros)
            .Where(string.IsNullOrEmpty(filtro) ?
                e => true :
                e => e.NombreEditora.Contains(filtro) || (e.Descripcion ?? "").Contains(filtro))
            .ToListAsync();
    }

    public override async Task<Editora?> ObtenerPorIdAsync(int id)
    {
        var editora = await context.Editoras
            .Where(x => !(x.Eliminado ?? false))
            .Include(l => l.Libros)
            .Where(l => l.CodigoEditora == id)
            .FirstOrDefaultAsync();

        return editora;
    }

    public async Task<IEnumerable<Editora>> BuscarEditorasAsync(string filtro)
    {
        return await context.Editoras
            .Where(x => !(x.Eliminado ?? false))
            .Where(string.IsNullOrEmpty(filtro) ?
                e => true :
                e => e.NombreEditora.Contains(filtro) || (e.Descripcion??"").Contains(filtro))
            .ToListAsync();
    }

}
