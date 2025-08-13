using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;
public class ErrorController : Controller
{
    [Route("Error/404")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }

    [Route("Error/{code}")]
    public IActionResult Error(int code)
    {
        Response.StatusCode = code;
        return View("Error");
    }
}
