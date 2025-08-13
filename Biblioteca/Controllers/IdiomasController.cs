using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class IdiomasController(ContextoBiblioteca context, IdiomaServicio service, LibroServicio libroServicio) : Controller
    {
        // GET: Idiomas
        public async Task<IActionResult> Index(string? filtro)
        {
            var idiomas = await service.ObtenerTodosAsync(filtro);

            ViewBag.Filtro = filtro;
            return View(idiomas);
        }

        [HttpGet("Idiomas/{id}/Libros")]
        public async Task<IActionResult> LibrosPorIdioma([FromRoute] int id, [FromQuery]string? filtro)
        {
            if (id <= 0)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id);

            if (idioma == null)
                return NotFound();

            var libros = (await libroServicio.ObtenerPorIdIdiomaAsync(id, filtro))
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
                ViewBag.Mensaje = "No se encontraron libros para este idioma.";
            }

            ViewBag.Idioma = idioma.NombreIdioma;
            ViewBag.CodigoIdioma = idioma.CodigoIdioma;
            ViewBag.Filtro = filtro;

            return View(libros);
        }

        // GET: Idiomas/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);

            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        // GET: Idiomas/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Idiomas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Idioma idioma)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(idioma);
                return RedirectToAction(nameof(Index));
            }
            return View(idioma);
        }

        // GET: Idiomas/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);
            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        // POST: Idiomas/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Idioma idioma)
        {
            if (id != idioma.CodigoIdioma)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(idioma);
            }

            try
            {
                await service.ActualizarAsync(idioma);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IdiomaExists(idioma.CodigoIdioma))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Idiomas/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);
            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool IdiomaExists(int id)
        {
            return context.Idiomas.Any(e => e.CodigoIdioma == id);
        }
    }
}
