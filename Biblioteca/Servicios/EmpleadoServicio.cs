using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;
using Biblioteca.Common;

namespace Biblioteca.Servicios;
public class EmpleadoServicio(ContextoBiblioteca context) : ServicioBase<Empleado>(context)
{
    public async Task<bool> ValidarCredenciales(string usuario, string contrasenia)
    {
        // Aqu� se implementar�a la l�gica para validar las credenciales
        // Por ejemplo, consultar la base de datos para verificar si el usuario existe y si la contrase�a es correcta.

        var empleado = await context.Empleados.FirstOrDefaultAsync(u => u.NombreUsuario == usuario);

        if (empleado == null)
        {
            return false; // Usuario no encontrado
        }

        return empleado.Contrasenia == Encryption.GetMD5(contrasenia);
    }

    public override async Task<List<Empleado>> ObtenerTodosAsync()
    {
        return await context.Empleados
            .Where(x => !(x.Eliminado ?? false))
            .Include(e => e.TandaLabor)
            .Include(e => e.Prestamos)
            .Include(e => e.Rol)
            .ToListAsync();
    }

    public override async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        var empleado = await context.Empleados
            .Where(x => !(x.Eliminado ?? false))
            .Include(e => e.TandaLabor)
            .Include(e => e.Prestamos)
            .Include(e=>e.Rol)
            .FirstOrDefaultAsync(e => e.CodigoEmpleado == id);

        // Cargar los libros y usuarios relacionados a trav�s de los pr�stamos
        foreach (var prestamo in empleado?.Prestamos ?? Enumerable.Empty<Prestamo>())
        {
            await context.Entry(prestamo).Reference(p => p.Libro).LoadAsync();
            await context.Entry(prestamo).Reference(p => p.Estudiante).LoadAsync();
        }

        return empleado;
    }

    public async Task<Empleado?> ObtenerEmpleadoPorNombreUsuario(string nombreUsuario)
    {
        return await context.Empleados
            .Where(x => !(x.Eliminado ?? false))
            .Include(e => e.Rol)
            .FirstOrDefaultAsync(e => e.NombreUsuario == nombreUsuario);
    }

    public override Task<Empleado> AgregarAsync(Empleado entidad)
    {
        // Validar que el nombre de usuario sea �nico
        if (context.Empleados.Any(e => e.NombreUsuario == entidad.NombreUsuario && !(e.Eliminado ?? false)))
        {
            throw new Exception("El nombre de usuario ya est� en uso.");
        }

        // Validar que la c�dula sea �nica
        if (context.Empleados.Any(e => e.Cedula == entidad.Cedula && !(e.Eliminado ?? false)))
        {
            throw new Exception("La c�dula ya est� en uso.");
        }

        // Validar que el c�digo de tanda sea v�lido
        if (!context.TandasLabor.Any(t => t.CodigoTanda == entidad.CodigoTanda && !(t.Eliminado ?? false)))
        {
            throw new Exception("El c�digo de tanda no es v�lido.");
        }

        // Validar que el c�digo de rol sea v�lido
        if (!context.Roles.Any(r => r.CodigoRol == entidad.CodigoRol && !(r.Eliminado ?? false)))
        {
            throw new Exception("El c�digo de rol no es v�lido.");
        }

        // Validar que la fecha de ingreso sea v�lida
        if (entidad.FechaIngreso.HasValue && entidad.FechaIngreso.Value > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Exception("La fecha de ingreso no puede ser futura.");
        }

        // Validar que el porcentaje de comisi�n sea v�lido
        if (entidad.PorcentajeComision.HasValue && (entidad.PorcentajeComision < 0 || entidad.PorcentajeComision > 100))
        {
            throw new Exception("El porcentaje de comisi�n debe estar entre 0 y 100.");
        }

        // Validar que la contrase�a tenga al menos 8 caracteres
        if (string.IsNullOrWhiteSpace(entidad.Contrasenia) || entidad.Contrasenia.Length < 8)
        {
            throw new Exception("La contrase�a debe tener al menos 8 caracteres.");
        }

        // Encriptar la contrase�a antes de guardarla
        entidad.Contrasenia = Encryption.GetMD5(entidad.Contrasenia);

        return base.AgregarAsync(entidad);
    }
}
