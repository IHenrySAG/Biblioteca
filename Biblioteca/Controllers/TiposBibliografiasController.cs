using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;

namespace Biblioteca.Controllers
{
    [Authorization("nadie")]
    public class TiposBibliografiasController(ContextoBiblioteca context, TipoBibliografiaServicio service) : Controller
    {
        // GET: TiposBibliografias
        public async Task<IActionResult> Index()
        {
            var tipos = await service.ObtenerTodosAsync();
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

        // GET: TiposBibliografias/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: TiposBibliografias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("CodigoBibliografia,NombreBibliografia,Descripcion,Estado")] TipoBibliografia tipo)
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
        public async Task<IActionResult> Editar(int id, [Bind("CodigoBibliografia,NombreBibliografia,Descripcion,Estado")] TipoBibliografia tipo)
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
