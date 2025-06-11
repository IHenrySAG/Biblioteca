using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;

namespace Biblioteca.Controllers
{
    public class EditorasController(ContextoBiblioteca context, EditoraServicio service) : Controller
    {
        // GET: Editoras
        public async Task<IActionResult> Index()
        {
            var editoras = await service.ObtenerTodosAsync();
            return View(editoras);
        }

        // GET: Editoras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);

            if (editora == null)
                return NotFound();

            return View(editora);
        }

        // GET: Editoras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Editoras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEditora,NombreEditora,Descripcion,Estado")] Editora editora)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(editora);
                return RedirectToAction(nameof(Index));
            }
            return View(editora);
        }

        // GET: Editoras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);
            if (editora == null)
                return NotFound();

            return View(editora);
        }

        // POST: Editoras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEditora,NombreEditora,Descripcion,Estado")] Editora editora)
        {
            if (id != editora.CodigoEditora)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(editora);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EditoraExists(editora.CodigoEditora))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editora);
        }

        // GET: Editoras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);
            if (editora == null)
                return NotFound();

            return View(editora);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool EditoraExists(int id)
        {
            return context.Editoras.Any(e => e.CodigoEditora == id);
        }
    }
}
