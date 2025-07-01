using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.BIBLIOTECARIO))]
    public class TiposBibliografiasController(ContextoBiblioteca context, TipoBibliografiaServicio service) : Controller
    {
        // GET: TiposBibliografias
        public async Task<IActionResult> Index()
        {
            var tipos = await service.ObtenerTodosAsync();
            return View(tipos);
        }

        // GET: TiposBibliografias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);

            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // GET: TiposBibliografias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposBibliografias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoBibliografia,NombreBibliografia,Descripcion,Estado")] TipoBibliografia tipo)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(tipo);
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        // GET: TiposBibliografias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        // POST: TiposBibliografias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoBibliografia,NombreBibliografia,Descripcion,Estado")] TipoBibliografia tipo)
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

        // GET: TiposBibliografias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tipo = await service.ObtenerPorIdAsync(id.Value);
            if (tipo == null)
                return NotFound();

            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
