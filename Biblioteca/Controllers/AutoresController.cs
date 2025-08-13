using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class AutoresController(
        ContextoBiblioteca context,
        AutorServicio service,
        LibroServicio libroServicio,
        ServicioBase<Idioma> servicioIdioma
    ) : Controller
    {
        // GET: Autores
        public async Task<IActionResult> Index(string? filtro)
        {
            var autores = await service.ObtenerTodosAsync(filtro);

            ViewBag.Filtro = filtro;
            return View(autores);
        }

        [HttpGet("Autores/{id}/Libros")]
        public async Task<IActionResult> LibrosPorAutor([FromRoute] int id, [FromQuery] string? filtro)
        {
            if (id <= 0)
                return NotFound();

            var autor = await service.ObtenerPorIdAsync(id);

            if (autor == null)
                return NotFound();

            var libros = (await libroServicio.ObtenerPorIdAutorAsync(id, filtro))
                .Select(l => new LibroVM
                {
                    CodigoLibro = l.CodigoLibro,
                    Titulo = l.Titulo,
                    SignaturaTopografica = l.SignaturaTopografica,
                    Isbn = l.Isbn,
                    AnioPublicacion = l.AnioPublicacion,
                    Ciencia = l.Ciencia,
                    Inventario = l.Inventario,
                    Idioma = l.Idioma.NombreIdioma,
                    Editora = l.Editora.NombreEditora
                }).ToList();

            if (libros == null || libros.Count == 0)
            {
                ViewBag.Mensaje = "No se encontraron libros para este Autor.";
            }

            ViewBag.Autor = autor.NombreAutor;
            ViewBag.CodigoAutor = autor.CodigoAutor;
            ViewBag.Filtro = filtro;

            return View(libros);
        }

        // GET: Autores/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await service.ObtenerPorIdAsync(id.Value);

            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // GET: Autores/Crear
        public async Task<IActionResult> Crear()
        {
            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                ViewBag.ErrorIdiomas = "No hay idiomas registrados. Por favor, registre al menos un idioma antes de registrar un autor.";

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View();
        }

        // POST: Autores/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("CodigoAutor,NombreAutor,CodigoIdioma,PaisOrigen,Estado")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(autor);
                return RedirectToAction(nameof(Index));
            }

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Crear", "Idiomas", new { RedirectedFrom = "CreateAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // GET: Autores/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var autor = await service.ObtenerPorIdAsync(id.Value);
            if (autor == null)
                return NotFound();

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Crear", "Idiomas", new { RedirectedFrom = "EditAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // POST: Autores/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("CodigoAutor,NombreAutor,CodigoIdioma,PaisOrigen,Estado")] Autor autor)
        {
            if (id != autor.CodigoAutor)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(autor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.CodigoAutor))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Crear", "Idiomas", new { RedirectedFrom = "EditAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // GET: Autores/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var autor = await service.ObtenerPorIdAsync(id.Value);
            if (autor == null)
                return NotFound();

            return View(autor);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BuscarAutorJson(string filtro)
        {
            return Json(await service.BuscarAutorAsync(filtro));
        }

        private bool AutorExists(int id)
        {
            return context.Autores.Any(a => a.CodigoAutor == id);
        }
    }
}
