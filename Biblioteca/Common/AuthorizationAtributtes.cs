using Biblioteca.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class AuthorizationAttribute : Attribute
    {
        public string[] RoleNames { get; }

        // This is a positional argument
        public AuthorizationAttribute(params string[] roleNames)
        {
            RoleNames = roleNames;
        }
    }

    public class AuthorizationFilter(IHttpContextAccessor _httpContextAccessor) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationAttributes = context.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizationAttribute>();

            if (!authorizationAttributes.Any())
                return;

            List<string> rolesEsperados = [];
            rolesEsperados.AddRange(authorizationAttributes.SelectMany(attr => attr.RoleNames));

            var http = context.HttpContext;
            string rolUsuario = http.Session.GetString("Rol");

            if (string.IsNullOrEmpty(rolUsuario))
            {
                http.Session.Clear();
                http.Response.Redirect("/");
            }

            if (!rolesEsperados.Contains(rolUsuario))
            {
                switch (rolUsuario)
                {
                    case nameof(ERoles.ADMIN): // Administrador
                        http.Response.Redirect("/Empleados/Index");
                        break;
                    case nameof(ERoles.BIBLIOTECARIO): // Bibliotecario
                        http.Response.Redirect("/Prestamos/Index");
                        break;
                    case nameof(ERoles.CATALOGADOR): // Catalogador
                        http.Response.Redirect("/Libros/Index");
                        break;
                    default:
                        http.Session.Clear();
                        http.Response.Redirect("/");
                        break;
                }
            }

            await Task.CompletedTask;
        }
    }
}
