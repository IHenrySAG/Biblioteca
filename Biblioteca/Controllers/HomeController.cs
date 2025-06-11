using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Presentacion.Web.Controllers;
public class HomeController : Controller
{

    [HttpGet("/")]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Libros");
    }

    [HttpGet("/ErrorTest")]
    public IActionResult Error()
    {
        return View();
    }

    [HttpGet("/Privacidad")]
    public IActionResult Privacy()
    {
        return View();
    }
}
