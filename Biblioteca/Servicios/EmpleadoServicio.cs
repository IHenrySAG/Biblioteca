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
        // Aquí se implementaría la lógica para validar las credenciales
        // Por ejemplo, consultar la base de datos para verificar si el usuario existe y si la contraseña es correcta.

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

        // Cargar los libros y usuarios relacionados a través de los préstamos
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
        // Validar que el nombre de usuario sea único
        if (context.Empleados.Any(e => e.NombreUsuario == entidad.NombreUsuario && !(e.Eliminado ?? false)))
        {
            throw new Exception("El nombre de usuario ya está en uso.");
        }

        // Validar que la cédula sea única
        if (context.Empleados.Any(e => e.Cedula == entidad.Cedula && !(e.Eliminado ?? false)))
        {
            throw new Exception("La cédula ya está en uso.");
        }

        // Validar que el código de tanda sea válido
        if (!context.TandasLabor.Any(t => t.CodigoTanda == entidad.CodigoTanda && !(t.Eliminado ?? false)))
        {
            throw new Exception("El código de tanda no es válido.");
        }

        // Validar que el código de rol sea válido
        if (!context.Roles.Any(r => r.CodigoRol == entidad.CodigoRol && !(r.Eliminado ?? false)))
        {
            throw new Exception("El código de rol no es válido.");
        }

        // Validar que la fecha de ingreso sea válida
        if (entidad.FechaIngreso.HasValue && entidad.FechaIngreso.Value > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new Exception("La fecha de ingreso no puede ser futura.");
        }

        // Validar que el porcentaje de comisión sea válido
        if (entidad.PorcentajeComision.HasValue && (entidad.PorcentajeComision < 0 || entidad.PorcentajeComision > 100))
        {
            throw new Exception("El porcentaje de comisión debe estar entre 0 y 100.");
        }

        // Validar que la contraseña tenga al menos 8 caracteres
        if (string.IsNullOrWhiteSpace(entidad.Contrasenia) || entidad.Contrasenia.Length < 8)
        {
            throw new Exception("La contraseña debe tener al menos 8 caracteres.");
        }

        // Encriptar la contraseña antes de guardarla
        entidad.Contrasenia = Encryption.GetMD5(entidad.Contrasenia);

        return base.AgregarAsync(entidad);
    }
}
