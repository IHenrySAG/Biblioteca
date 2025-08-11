using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Libro : EntidadBase
{
    public int CodigoLibro { get; set; }
    public string? Titulo { get; set; }
    public string? SignaturaTopografica { get; set; }
    public string? Isbn { get; set; }
    public int CodigoEditora { get; set; }
    public int AnioPublicacion { get; set; }
    public string? Ciencia { get; set; }
    public int CodigoIdioma { get; set; }
    public int Inventario { get; set; }

    public Editora? Editora { get; set; }
    public Idioma? Idioma { get; set; }
    public ICollection<LibroBibliografia>? LibrosBibliografias { get; set; }
    public ICollection<LibroAutor>? LibrosAutores { get; set; }
    public ICollection<Prestamo>? Prestamos { get; set; }
}