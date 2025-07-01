using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Presentacion.Web.Views.Home
{
    public class Error401 : PageModel
    {
        private readonly ILogger<Error401> _logger;

        public Error401(ILogger<Error401> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
