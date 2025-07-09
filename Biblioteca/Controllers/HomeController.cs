using Biblioteca.Common;
using Biblioteca.Pages;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;
public class HomeController(EmpleadoServicio _servicio) : Controller
{

    [NoAuthorization]
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [NoAuthorization]
    [HttpPost("/")]
    public async Task<IActionResult> Index(IndexModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ErrorMessage = "Por favor, complete todos los campos.";
            return View(model);
        }

        var credencialesValidas = await _servicio.ValidarCredenciales(model.Username, model.Password);

        if (!credencialesValidas)
        {
            model.ErrorMessage = "Usuario o contraseña incorrectos.";
            return View(model);
        }

        var empleado = await _servicio.ObtenerEmpleadoPorNombreUsuario(model.Username);

        HttpContext.Session.SetInt32("CodigoEmpleado", empleado!.CodigoEmpleado);
        HttpContext.Session.SetString("Empleado", empleado!.NombreUsuario);
        HttpContext.Session.SetString("Rol", empleado!.Rol!.NombreRol);

        switch (empleado.CodigoRol)
        {
            case (int)ERoles.ADMIN: // Administrador
                return RedirectToAction("Index", "Empleados");
            case (int)ERoles.BIBLIOTECARIO: // Bibliotecario
                return RedirectToAction("Index", "Prestamos");
            case (int)ERoles.CATALOGADOR: // Catalogador
                return RedirectToAction("Index", "Libros");
            default:
                model.ErrorMessage = "Rol no reconocido, contacte con el administrador del sistema.";
                return View(model);
        }
    }

    [HttpGet("/CerrarSesion")]
    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpGet("/Error404")]
    public IActionResult Error404()
    {
        return View();
    }

    [HttpGet("/Error401")]
    public IActionResult Error401()
    {
        return View();
    }

    [HttpGet("/Error403")]
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public IActionResult Error403()
    {
        return View();
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
