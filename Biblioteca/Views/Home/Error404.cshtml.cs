using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Presentacion.Web.Views.Home
{
    public class Error404 : PageModel
    {
        private readonly ILogger<Error404> _logger;

        public Error404(ILogger<Error404> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
