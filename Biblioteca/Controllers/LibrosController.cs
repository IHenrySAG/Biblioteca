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
using Biblioteca.Model.DTOs;
using Biblioteca.Model.ViewModel;


namespace Biblioteca.Controllers
{
    [Authorization(nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR), nameof(ERoles.BIBLIOTECARIO))]
    public class LibrosController(ContextoBiblioteca context, LibroServicio service, ServicioBase<Idioma> servicioIdioma, ServicioBase<Editora> servicioEditoras, ServicioBase<Autor> servicioAutores, ServicioBase<TipoBibliografia> servicioBibliografia) : Controller
    {
        // GET: Libros
        public async Task<IActionResult> Index(string? filtro)
        {
            var libros =filtro is null? await service.ObtenerTodosAsync(): await service.ObtenerConFiltroAsync(filtro);

            ViewBag.Filtro = filtro;

            ViewBag.EditarLibros = new string[] { nameof(ERoles.ADMIN), nameof(ERoles.CATALOGADOR) }
                .Contains(HttpContext.Session.GetString("Rol"));

            var librosVM=libros.Select(x => new LibroVM
            {
                CodigoLibro = x.CodigoLibro,
                Titulo = x.Titulo,
                SignaturaTopografica = x.SignaturaTopografica,
                Isbn = x.Isbn,
                AnioPublicacion = x.AnioPublicacion,
                Editora = x.Editora?.NombreEditora ?? string.Empty,
                Ciencia = x.Ciencia,
                Idioma = x.Idioma.NombreIdioma,
                Autores = x.LibrosAutores.Select(la => la.Autor?.NombreAutor ?? string.Empty).Where(a => !string.IsNullOrEmpty(a)).ToList(),
                Inventario = x.Inventario
            });

            return View(librosVM);
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
        public async Task<IActionResult> Edit(int id, [Bind("CodigoLibro,Titulo,SignaturaTopografica,Isbn,CodigoEditora,AnioPublicacion,Ciencia,CodigoIdioma,Inventario")] Libro libro, int[] autores, List<TipoBibliografia> bibliografias)
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

        // GET: Libros/ExportarLibroCsv/5
        public async Task<IActionResult> ExportarLibroCsv(int id)
        {
            var libro = await service.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("CodigoLibro;Titulo;SignaturaTopografica;Isbn;CodigoEditora;AnioPublicacion;Ciencia;CodigoIdioma;Eliminado");
            sb.AppendLine($"{libro.CodigoLibro};\"{libro.Titulo}\";\"{libro.SignaturaTopografica}\";\"{libro.Isbn}\";{libro.CodigoEditora};{libro.AnioPublicacion};\"{libro.Ciencia}\";{libro.CodigoIdioma};{libro.Eliminado}");
            var fileName = string.IsNullOrWhiteSpace(libro.Titulo) ? "Libro.csv" : libro.Titulo;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            fileName += ".csv";
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", fileName);
        }

        // GET: Libros/ExportarLibrosCsv
        public async Task<IActionResult> ExportarLibrosCsv()
        {
            var libros = await service.ObtenerTodosAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Codigo Libro;Titulo;Signatura Topografica;Isbn;Codigo Editora;Anio Publicacion;Ciencia;Idioma");

            foreach (var libro in libros)
            {
                sb.AppendLine($"{libro.CodigoLibro};\"{libro.Titulo}\";\"{libro.SignaturaTopografica}\";\"{libro.Isbn}\";{libro.CodigoEditora};{libro.AnioPublicacion};\"{libro.Ciencia}\";{libro.Idioma.NombreIdioma}");
            }
            var fileName = $"Lista_Libros_{DateTime.Now.Date:yyyy_MM_dd}";
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            fileName += ".csv";
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", fileName);
        }

        // GET: Libros/ExportarLibroXml/5
        public async Task<IActionResult> ExportarLibroXml(int id)
        {
            var libro = await service.ObtenerPorIdAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            var dto = new Biblioteca.Model.DTOs.LibroExportDto
            {
                CodigoLibro = libro.CodigoLibro,
                Titulo = libro.Titulo,
                SignaturaTopografica = libro.SignaturaTopografica,
                Isbn = libro.Isbn,
                CodigoEditora = libro.CodigoEditora,
                AnioPublicacion = libro.AnioPublicacion,
                Ciencia = libro.Ciencia,
                CodigoIdioma = libro.CodigoIdioma,
                Eliminado = libro.Eliminado
            };
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Biblioteca.Model.DTOs.LibroExportDto));
            using (var ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, dto);
                ms.Position = 0;
                string titulo = string.IsNullOrWhiteSpace(libro.Titulo) ? "Libro" : libro.Titulo;
                foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                {
                    titulo = titulo.Replace(c, '-');
                }
                var fileName = $"{titulo}.txt";
                return File(ms.ToArray(), "text/plain", fileName);
            }
        }

        // GET: Libros/ExportarLibroXml/5
        public async Task<IActionResult> ExportarLibrosXml()
        {
            var libros = await service.ObtenerTodosAsync();
            if (!libros.Any())
            {
                return NotFound();
            }

            var dtos = libros.Select(libro => new LibroExportDto
            {
                CodigoLibro = libro.CodigoLibro,
                Titulo = libro.Titulo,
                SignaturaTopografica = libro.SignaturaTopografica,
                Isbn = libro.Isbn,
                CodigoEditora = libro.CodigoEditora,
                AnioPublicacion = libro.AnioPublicacion,
                Ciencia = libro.Ciencia,
                CodigoIdioma = libro.CodigoIdioma,
                Eliminado = libro.Eliminado
            }).ToList();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LibroExportDto>));

            using var ms = new System.IO.MemoryStream();
            serializer.Serialize(ms, dtos);
            ms.Position = 0;
            string titulo = $"Libros_Exportados_{DateTime.Now.Date:yyyy_MM_dd}";
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                titulo = titulo.Replace(c, '-');
            }
            var fileName = $"{titulo}.xml";
            return File(ms.ToArray(), "text/plain", fileName);
        }

        [HttpGet]
        public async Task<JsonResult> BuscarLibroJson(string filtro)
        {
            var libros = await service.ObtenerConFiltroAsync(filtro);

            var librosVM = libros.Select(x => new LibroVM
            {
                CodigoLibro = x.CodigoLibro,
                Titulo = x.Titulo,
                SignaturaTopografica = x.SignaturaTopografica,
                Isbn = x.Isbn,
                AnioPublicacion = x.AnioPublicacion,
                Editora = x.Editora?.NombreEditora ?? string.Empty,
                Ciencia = x.Ciencia,
                Autores = x.LibrosAutores.Select(la => la.Autor?.NombreAutor ?? string.Empty).Where(a => !string.IsNullOrEmpty(a)).ToList(),
                Idioma = x.Idioma.NombreIdioma,
                Inventario = x.Inventario
            });
            return Json(librosVM);
        }

        private bool LibroExists(int id)
        {
            return context.Libros.Any(e => e.CodigoLibro == id);
        }
    }
}
