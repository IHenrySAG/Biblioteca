using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;
using Microsoft.IdentityModel.Tokens;
using Biblioteca.Model.ViewModel;

namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class EditorasController(ContextoBiblioteca context, EditoraServicio service, LibroServicio libroServicio) : Controller
    {
        // GET: Editoras
        public async Task<IActionResult> Index(string? filtro)
        {
            var editoras = await service.ObtenerTodosAsync(filtro);

            ViewBag.Filtro = filtro;
            return View(editoras);
        }

        [HttpGet("Editoras/{id}/Libros")]
        public async Task<IActionResult> LibrosPorEditora([FromRoute] int id, [FromQuery] string? filtro)
        {
            if (id <= 0)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id);

            if (editora == null)
                return NotFound();

            var libros = (await libroServicio.ObtenerPorIdEditoraAsync(id, filtro))
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
                ViewBag.Mensaje = "No se encontraron libros para esta editora.";
            }

            ViewBag.Editora = editora.NombreEditora;
            ViewBag.CodigoEditora = editora.CodigoEditora;
            ViewBag.Filtro = filtro;

            return View(libros);
        }

        // GET: Editoras/Detalles/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);

            if (editora == null)
                return NotFound();

            return View(editora);
        }

        // GET: Editoras/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Editoras/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("CodigoEditora,NombreEditora,Descripcion,Estado")] Editora editora)
        {
            if (ModelState.IsValid)
            {
                await service.AgregarAsync(editora);
                return RedirectToAction(nameof(Index));
            }
            return View(editora);
        }

        // GET: Editoras/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);
            if (editora == null)
                return NotFound();

            return View(editora);
        }

        // POST: Editoras/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("CodigoEditora,NombreEditora,Descripcion,Estado")] Editora editora)
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

        // GET: Editoras/Eliminar/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
                return NotFound();

            var editora = await service.ObtenerPorIdAsync(id.Value);
            if (editora == null)
                return NotFound();

            return View(editora);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            await service.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerEditorasJson(int cantidad)
        {
            var editoras = await service.ObtenerTodosAsync();
            return Json(new { success = true, data = editoras.Take(cantidad) });
        }

        [HttpGet]
        public async Task<JsonResult> BuscarEditoraJson(string filtro)
        {
            var editoras= await service.BuscarEditorasAsync(filtro);
            return Json(editoras);
        }

        // POST: Editoras/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarEditoraJson(string nombre, string descripcion)
        {
            if (nombre.Trim().IsNullOrEmpty())
            {
                return BadRequest("El nombre es requerido");
            }

            var editora = await service.AgregarAsync(new()
            {
                NombreEditora = nombre,
                Descripcion = descripcion
            });

            return Json(editora);
        }

        private bool EditoraExists(int id)
        {
            return context.Editoras.Any(e => e.CodigoEditora == id);
        }
    }
}
