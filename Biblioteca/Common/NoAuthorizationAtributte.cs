using Biblioteca.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class NoAuthorizationAttribute : Attribute
    {

    }

    public class NoAuthorizationFilter(IHttpContextAccessor _httpContextAccessor) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasNoAuthorizationAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<NoAuthorizationAttribute>()
            .Any();

            if (!hasNoAuthorizationAttribute)
                return;

            var http = context.HttpContext;
            string rol = http.Session.GetString("Rol");

            if (!string.IsNullOrEmpty(rol))
            {
                switch (rol)
                {
                    case nameof(ERoles.ADMIN): // Administrador
                        http.Response.Redirect("/Empleados");
                        break;
                    case nameof(ERoles.BIBLIOTECARIO): // Bibliotecario
                        http.Response.Redirect("/Prestamos");
                        break;
                    case nameof(ERoles.CATALOGADOR): // Catalogador
                        http.Response.Redirect("/Libros");
                        break;
                    default:
                        http.Session.Clear();
                        http.Response.Redirect("/");
                        break;
                }
                
                await Task.CompletedTask;
            }
            else { http.Session.Clear(); }

            await Task.CompletedTask;
        }
    }


}


