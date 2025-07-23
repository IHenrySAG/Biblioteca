using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class AutoresController(
        ContextoBiblioteca context,
        AutorServicio service,
        ServicioBase<Idioma> servicioIdioma
    ) : Controller
    {
        // GET: Autores
        public async Task<IActionResult> Index()
        {
            var autores = await service.ObtenerTodosAsync();
            return View(autores);
        }

        // GET: Autores/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Autores/Create
        public async Task<IActionResult> Create()
        {
            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Create", "Idiomas", new { RedirectedFrom = "CreateAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View();
        }

        // POST: Autores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoAutor,NombreAutor,CodigoIdioma,PaisOrigen,Estado")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(autor);
                return RedirectToAction(nameof(Index));
            }

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Create", "Idiomas", new { RedirectedFrom = "CreateAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // GET: Autores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var autor = await service.ObtenerPorIdAsync(id.Value);
            if (autor == null)
                return NotFound();

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Create", "Idiomas", new { RedirectedFrom = "EditAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // POST: Autores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoAutor,NombreAutor,CodigoIdioma,PaisOrigen,Estado")] Autor autor)
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
                return RedirectToAction("Create", "Idiomas", new { RedirectedFrom = "EditAutor" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View(autor);
        }

        // GET: Autores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var autor = await service.ObtenerPorIdAsync(id.Value);
            if (autor == null)
                return NotFound();

            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
