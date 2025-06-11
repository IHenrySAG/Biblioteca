using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;

namespace Biblioteca.Controllers
{
    public class TandasLaborController(
        ContextoBiblioteca context,
        TandaLaborServicio service
    ) : Controller
    {
        // GET: TandasLabor
        public async Task<IActionResult> Index()
        {
            var tandas = await service.ObtenerTodosAsync();
            return View(tandas);
        }

        // GET: TandasLabor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tanda = await service.ObtenerPorIdAsync(id.Value);

            if (tanda == null)
                return NotFound();

            return View(tanda);
        }

        // GET: TandasLabor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TandasLabor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoTanda,NombreTanda,HoraInicio,HoraFin,Estado")] TandaLabor tanda)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(tanda);
                return RedirectToAction(nameof(Index));
            }
            return View(tanda);
        }

        // GET: TandasLabor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tanda = await service.ObtenerPorIdAsync(id.Value);
            if (tanda == null)
                return NotFound();

            return View(tanda);
        }

        // POST: TandasLabor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoTanda,NombreTanda,HoraInicio,HoraFin,Estado")] TandaLabor tanda)
        {
            if (id != tanda.CodigoTanda)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await service.ActualizarAsync(tanda);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TandaLaborExists(tanda.CodigoTanda))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tanda);
        }

        // GET: TandasLabor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tanda = await service.ObtenerPorIdAsync(id.Value);
            if (tanda == null)
                return NotFound();

            return View(tanda);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TandaLaborExists(int id)
        {
            return context.TandasLabor.Any(t => t.CodigoTanda == id);
        }
    }
}
