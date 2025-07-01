using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages
{
    public class IndexModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? ErrorMessage { get; set; }

    }
}
