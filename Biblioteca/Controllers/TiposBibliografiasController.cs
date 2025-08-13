using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class TiposBibliografiasController(ContextoBiblioteca context, TipoBibliografiaServicio service, LibroServicio libroServicio) : Controller
    {
        // GET: TiposBibliografias
        public async Task<IActionResult> Index(string? filtro)
        {
            var tipos = await service.ObtenerTodosAsync(filtro);

            if (!string.IsNullOrEmpty(filtro))
            {
                ViewBag.Filtro = filtro;
            }
            return View(tipos);
        }

        // GET: TiposBibliografias/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);

            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        [HttpGet("TiposBibliografias/{id}/Libros")]
        public async Task<IActionResult> LibrosPorBibliografia([FromRoute] int id, [FromQuery] string? filtro)
        {
            if (id <= 0)
                return NotFound();

            var tipoBibliografia = await service.ObtenerPorIdAsync(id);

            if (tipoBibliografia == null)
                return NotFound();

            var libros = (await libroServicio.ObtenerPorIdBibliografiaAsync(id, filtro))
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
                    Editora = l.Editora.NombreEditora,
                    TipoBibliografia = l.TipoBibliografia.NombreBibliografia
                }).ToList();

            if (libros == null || libros.Count == 0)
            {
                ViewBag.Mensaje = "No se encontraron libros para esta bibliografia.";
            }

            ViewBag.Bibliografia = tipoBibliografia.NombreBibliografia;
            ViewBag.CodigoBibliografia = tipoBibliografia.CodigoBibliografia;
            ViewBag.Filtro = filtro;

            return View(libros);
        }

        // GET: TiposBibliografias/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: TiposBibliografias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoBibliografia tipo)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: TiposBibliografias/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: TiposBibliografias/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, TipoBibliografia tipo)
        {
            if (id != tipo.CodigoBibliografia)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(tipo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoBibliografiaExists(tipo.CodigoBibliografia))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: TiposBibliografias/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TipoBibliografiaExists(int id)
        {
            return context.TiposBibliografias.Any(e => e.CodigoBibliografia == id);
        }
    }
}
