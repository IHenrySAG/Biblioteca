using Biblioteca.Negocio.Logica.Servicios;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Negocio.Logica;
public static class DependencyInjection
{
    public static IServiceCollection AddNegocioLogica(this IServiceCollection services)
    {
        // Aquí puedes registrar tus servicios de negocio
        // Por ejemplo:
        // services.AddScoped<ServicioBase<TipoBibliografia>>();
        // services.AddScoped<ServicioBase<Editora>>();
        // services.AddScoped<ServicioBase<Idioma>>();
        // services.AddScoped<ServicioBase<Autor>>();
        // services.AddScoped<ServicioBase<Libro>>();

        services.AddTransient(typeof(ServicioBase<>), typeof(ServicioBase<>));
        return services;
    }
}
