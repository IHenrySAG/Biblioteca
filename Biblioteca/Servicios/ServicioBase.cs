using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca.Servicios;
public class ServicioBase<T>(ContextoBiblioteca context) 
    where T : EntidadBase
{
    public virtual async Task<IEnumerable<T>> ObtenerTodosAsync(string filtro=null)
    {
        return await context.Set<T>()
            .Where(x=>!(x.Eliminado??false))
            .ToListAsync();
    }

    public IQueryable<T> Query()
    {
        return context.Set<T>();
    }

    public virtual async Task<T?> ObtenerPorIdAsync(int id)
    {
        var entidad= await context.Set<T>().FindAsync(id);
        if (entidad?.Eliminado ?? false) return null;
        return entidad;
    }

    public virtual async Task<T> AgregarAsync(T entidad)
    {
        context.Set<T>().Add(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    public virtual async Task<T> ActualizarAsync(T entidad)
    {
        context.Set<T>().Update(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    public virtual async Task<bool> EliminarAsync(int id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad == null)
        {
            return false;
        }

        entidad.Eliminado = true; // Marcar como eliminado
        context.Set<T>().Update(entidad);
        await context.SaveChangesAsync();
        return true;
    }
}
