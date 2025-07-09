using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class TandaLaborServicio(ContextoBiblioteca context) : ServicioBase<TandaLabor>(context)
{
    public override async Task<IEnumerable<TandaLabor>> ObtenerTodosAsync()
    {
        return await context.TandasLabor
            .Where(x => !(x.Eliminado ?? false))
            .Include(t => t.Empleados)
            .ToListAsync();
    }

    public override async Task<TandaLabor?> ObtenerPorIdAsync(int id)
    {
        var tanda = await context.TandasLabor
            .Where(x => !(x.Eliminado ?? false))
            .Include(t => t.Empleados)
            .FirstOrDefaultAsync(t => t.CodigoTanda == id);

        return tanda;
    }
}
