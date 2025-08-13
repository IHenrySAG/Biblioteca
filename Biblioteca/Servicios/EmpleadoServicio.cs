using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteca.Model;
using Biblioteca.Common;
using Biblioteca.Model.ViewModel;

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

    public override async Task<IEnumerable<Empleado>> ObtenerTodosAsync(string filtro=null)
    {
        return await context.Empleados
            .AsNoTrackingWithIdentityResolution()
            .Include(e => e.TandaLabor)
            .Include(e => e.Prestamos)
            .Include(e => e.Rol)
            .Where(x => !(x.Eliminado ?? false))
            .Where(string.IsNullOrEmpty(filtro) ?
                e => true :
                e => e.Nombre.Contains(filtro) || e.Apellido.Contains(filtro) || e.Cedula.Contains(filtro) || e.Rol.NombreRol.Contains(filtro) || e.TandaLabor.NombreTanda.Contains(filtro))
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
            throw new InternalException("El nombre de usuario ya está en uso.");
        }

        // Validar que la cédula sea única
        if (context.Empleados.Any(e => e.Cedula == entidad.Cedula && !(e.Eliminado ?? false)))
        {
            throw new InternalException("La cédula ya está en uso.");
        }

        // Validar que el código de tanda sea válido
        if (!context.TandasLabor.Any(t => t.CodigoTanda == entidad.CodigoTanda && !(t.Eliminado ?? false)))
        {
            throw new InternalException("El código de tanda no es válido.");
        }

        // Validar que el código de rol sea válido
        if (!context.Roles.Any(r => r.CodigoRol == entidad.CodigoRol && !(r.Eliminado ?? false)))
        {
            throw new InternalException("El código de rol no es válido.");
        }

        // Validar que la fecha de ingreso sea válida
        if (entidad.FechaIngreso.HasValue && entidad.FechaIngreso.Value > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InternalException("La fecha de ingreso no puede ser futura.");
        }

        // Validar que el porcentaje de comisión sea válido
        if (entidad.PorcentajeComision.HasValue && (entidad.PorcentajeComision < 0 || entidad.PorcentajeComision > 100))
        {
            throw new InternalException("El porcentaje de comisión debe estar entre 0 y 100.");
        }

        // Validar que la contraseña tenga al menos 8 caracteres
        if (string.IsNullOrWhiteSpace(entidad.Contrasenia) || entidad.Contrasenia.Length < 8)
        {
            throw new InternalException("La contraseña debe tener al menos 8 caracteres.");
        }

        // Encriptar la contraseña antes de guardarla
        entidad.Contrasenia = Encryption.GetMD5(entidad.Contrasenia);

        return base.AgregarAsync(entidad);
    }

    public async Task<Empleado> ActualizarAsync(EditarEmpleadoVM entidad)
    {
        var empleado = await ObtenerPorIdAsync(entidad.CodigoEmpleado);
        if (empleado == null)
        {
            throw new Exception("Empleado no encontrado.");
        }

        // Validar que el nombre de usuario sea único
        if (context.Empleados.Any(e => e.NombreUsuario == empleado.NombreUsuario && e.CodigoEmpleado != empleado.CodigoEmpleado && !(e.Eliminado ?? false)))
        {
            throw new InternalException("El nombre de usuario ya está en uso.");
        }

        // Validar que la cédula sea única
        if (context.Empleados.Any(e => e.Cedula == entidad.Cedula && e.CodigoEmpleado != empleado.CodigoEmpleado && !(e.Eliminado ?? false)))
        {
            throw new InternalException("La cédula ya está en uso.");
        }

        // Validar que el código de tanda sea válido
        if (!context.TandasLabor.Any(t => t.CodigoTanda == entidad.CodigoTanda && !(t.Eliminado ?? false)))
        {
            throw new InternalException("El código de tanda no es válido.");
        }

        // Validar que el código de rol sea válido
        if (!context.Roles.Any(r => r.CodigoRol == entidad.CodigoRol && !(r.Eliminado ?? false)))
        {
            throw new InternalException("El código de rol no es válido.");
        }

        // Validar que la fecha de ingreso sea válida
        if (entidad.FechaIngreso.HasValue && entidad.FechaIngreso.Value > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InternalException("La fecha de ingreso no puede ser futura.");
        }

        // Actualizar los campos del empleado
        empleado.CodigoEmpleado = entidad.CodigoEmpleado;
        empleado.Nombre = entidad.Nombre;
        empleado.Apellido = entidad.Apellido;
        empleado.Cedula = entidad.Cedula;
        empleado.CodigoTanda = entidad.CodigoTanda;
        empleado.FechaIngreso = entidad.FechaIngreso;
        empleado.NombreUsuario = entidad.NombreUsuario;
        empleado.CodigoRol = entidad.CodigoRol;


        return await base.ActualizarAsync(empleado);
    }
}
