using Biblioteca.Datos.Persistencia;
using Biblioteca.Datos.Persistencia.Dominio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Logica.Servicios;
public class ServicioBase<T>(ContextoBiblioteca context) 
    where T : EntidadBase
{
    public async Task<List<T>> ObtenerTodosAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T> AgregarAsync(T entidad)
    {
        context.Set<T>().Add(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    public async Task<T> ActualizarAsync(T entidad)
    {
        context.Set<T>().Update(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var entidad = await ObtenerPorIdAsync(id);
        if (entidad == null)
        {
            return false;
        }

        entidad.Estado = false; // Marcar como eliminado
        context.Set<T>().Update(entidad);
        await context.SaveChangesAsync();
        return true;
    }
}
