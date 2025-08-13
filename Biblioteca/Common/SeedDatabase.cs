namespace Biblioteca.Common;

public class SeedDatabase
{
    public static async Task SeedAdmin(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ContextoBiblioteca>();

        if (!context.Empleados.Any(e => e.NombreUsuario == "admin"))
        {
            var admin = new Model.Empleado
            {
                Nombre = "Administrador",
                Apellido = "Sistema",
                Cedula = "00123456789",
                CodigoTanda = 1, // Asumiendo que la tanda laboral con código 1 existe
                NombreUsuario = "admin",
                Contrasenia = Encryption.GetMD5("Admin123"),
                CodigoRol = (int)ERoles.ADMIN
            };

            await context.Empleados.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
