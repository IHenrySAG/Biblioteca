using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Persistencia;
public static class DependencyInjection
{
    public static void AddPersistencia(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración de la cadena de conexión
        var connectionString = configuration.GetConnectionString("Biblioteca");

        // Configuración del contexto de la base de datos
        services.AddDbContext<ContextoBiblioteca>(options =>
            options.UseSqlServer(connectionString));

        // Registro de repositorios y servicios adicionales si es necesario
        // services.AddScoped<ITuRepositorio, TuRepositorio>();
        // services.AddScoped<ITuServicio, TuServicio>();
    }
}
