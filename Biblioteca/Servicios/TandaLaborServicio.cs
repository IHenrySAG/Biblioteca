using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class TandaLaborServicio(ContextoBiblioteca context) : ServicioBase<TandaLabor>(context)
{
    public override async Task<IEnumerable<TandaLabor>> ObtenerTodosAsync(string filtro=null)
    {
        return await context.TandasLabor
            .AsNoTrackingWithIdentityResolution()
            .Where(x => !(x.Eliminado ?? false))
            .Include(t => t.Empleados)
            .Where(string.IsNullOrEmpty(filtro) ?
                t => true :
                t => t.NombreTanda.Contains(filtro) ||
                     t.HoraInicio.ToString().Contains(filtro) ||
                     t.HoraFin.ToString().Contains(filtro))
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
