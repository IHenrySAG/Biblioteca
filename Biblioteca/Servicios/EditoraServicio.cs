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
    public override async Task<List<Editora>> ObtenerTodosAsync()
    {
        return await context.Editoras
            .Where(x=>!(x.Eliminado??false))
            .Include(l => l.Libros)
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

}
