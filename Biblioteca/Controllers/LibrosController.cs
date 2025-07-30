using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Model;
using Biblioteca.Servicios;
using Biblioteca.Common;


namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class LibrosController(ContextoBiblioteca context, LibroServicio service, ServicioBase<Idioma> servicioIdioma, ServicioBase<Editora> servicioEditoras, ServicioBase<Autor> servicioAutores, ServicioBase<TipoBibliografia> servicioBibliografia) : Controller
    {
        // GET: Libros
        public async Task<IActionResult> Index()
        {
            var libros = await service.ObtenerTodosAsync();

            ViewBag.EditarLibros = new string[] { nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR) }
                .Contains(HttpContext.Session.GetString("Rol"));

            return View(libros);
        }

        // GET: Libros/Buscar
        public async Task<IActionResult> Buscar(string filtro)
        {
            var libros = await service.ObtenerConFiltroAsync(filtro);

            ViewBag.EditarLibros = new string[] { nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR) }
                .Contains(HttpContext.Session.GetString("Rol"));
            ViewBag.Filtro = filtro;

            return View(libros);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await service.ObtenerPorIdAsync(id.Value);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libros/Create
        public async Task<IActionResult> Create()
        {
            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                ViewBag.ErrorIdiomas = "No hay idiomas registrados. Por favor, registre al menos un idioma antes de crear un libro.";

            var editoras = await servicioEditoras.ObtenerTodosAsync();

            if (!editoras.Any())
                ViewBag.ErrorEditoras = "No hay editoras registradas. Por favor, registre al menos una editora antes de crear un libro.";

            var autores=await servicioAutores.ObtenerTodosAsync();

            if (!autores.Any())
                ViewBag.ErrorAutores = "No hay autores registrados. Por favor, registre al menos un autor antes de crear un libro.";

            var bibliografias = await servicioBibliografia.ObtenerTodosAsync();

            if (!bibliografias.Any())
                ViewBag.ErrorBibliografias = "No hay bibliografías registradas. Por favor, registre al menos una bibliografía antes de crear un libro.";

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            return View();
        }

        // POST: Libros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoLibro,Titulo,SignaturaTopografica,Isbn,CodigoEditora,AnioPublicacion,Ciencia,CodigoIdioma")] Libro libro, int[] autores, List<TipoBibliografia> bibliografias)
        {
            if (!ModelState.IsValid)
            {
                var idiomas = await servicioIdioma.ObtenerTodosAsync();

                if (!idiomas.Any())
                    return RedirectToAction("Index", "Idiomas", new { RedirectedFrom = "CreateBook" });

                ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
                return View(libro);
            }

            libro.LibrosAutores ??= [];

            foreach(int codigoAutor in autores)
            {
                libro.LibrosAutores.Add(new()
                {
                    CodigoAutor = codigoAutor
                });
            }

            libro.LibrosBibliografias ??= [];

            foreach(var biblio in bibliografias)
            {
                libro.LibrosBibliografias.Add(new()
                {
                    TipoBibliografia=biblio
                });
            }

            await service.AgregarAsync(libro);
            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var libro = await service.ObtenerPorIdAsync(id ?? 0);
            if (libro == null)
            {
                return NotFound();
            }

            var idiomas = await servicioIdioma.ObtenerTodosAsync();

            if (!idiomas.Any())
                return RedirectToAction("Index", "Idiomas", new { RedirectedFrom = "CreateBook" });

            var editoras = await servicioEditoras.ObtenerTodosAsync();

            if (!editoras.Any())
                return RedirectToAction("Index", "Editoras", new { RedirectedFrom = "CreateBook" });

            ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
            ViewData["Editoras"] = new SelectList(editoras, "CodigoEditora", "NombreEditora");
            return View(libro);
        }

        // POST: Libros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoLibro,Titulo,SignaturaTopografica,Isbn,CodigoEditora,AnioPublicacion,Ciencia,CodigoIdioma")] Libro libro, int[] autores, List<TipoBibliografia> bibliografias)
        {
            if (id != libro.CodigoLibro)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var idiomas = await servicioIdioma.ObtenerTodosAsync();

                if (!idiomas.Any())
                    return RedirectToAction("Index", "Idiomas", new { RedirectedFrom = "CreateBook" });

                ViewData["Idiomas"] = new SelectList(idiomas, "CodigoIdioma", "NombreIdioma");
                return View(libro);
            }

            try
            {
                libro.LibrosAutores = [];

                foreach (int codigoAutor in autores)
                {
                    libro.LibrosAutores.Add(new()
                    {
                        CodigoAutor = codigoAutor
                    });
                }

                libro.LibrosBibliografias = [];

                foreach (var biblio in bibliografias)
                {
                    libro.LibrosBibliografias.Add(new()
                    {
                        TipoBibliografia = biblio
                    });
                }
                await service.ActualizarAsync(libro);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(libro.CodigoLibro))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await service.ObtenerPorIdAsync(id ?? 0);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // Fix for CS1503: Argument 1: cannot convert from 'Biblioteca.Model.Libro' to 'int'  
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.EliminarAsync(id); // Pass the 'id' instead of 'libro'  
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return context.Libros.Any(e => e.CodigoLibro == id);
        }
    }
}
