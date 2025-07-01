using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR))]
    public class IdiomasController(ContextoBiblioteca context, IdiomaServicio service) : Controller
    {
        // GET: Idiomas
        public async Task<IActionResult> Index()
        {
            var idiomas = await service.ObtenerTodosAsync();
            return View(idiomas);
        }

        // GET: Idiomas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);

            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        // GET: Idiomas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Idiomas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoIdioma,NombreIdioma,Estado")] Idioma idioma)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(idioma);
                return RedirectToAction(nameof(Index));
            }
            return View(idioma);
        }

        // GET: Idiomas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);
            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        // POST: Idiomas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoIdioma,NombreIdioma,Estado")] Idioma idioma)
        {
            if (id != idioma.CodigoIdioma)
                return NotFound();

            if (ModelState.IsValid)
            {
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
            return View(idioma);
        }

        // GET: Idiomas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var idioma = await service.ObtenerPorIdAsync(id.Value);
            if (idioma == null)
                return NotFound();

            return View(idioma);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
