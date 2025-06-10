using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Idioma : EntidadBase
{
    public int CodigoIdioma { get; set; }
    public string NombreIdioma { get; set; } = null!;

    public ICollection<Autor>? Autores { get; set; }
    public ICollection<Libro>? Libros { get; set; }
}