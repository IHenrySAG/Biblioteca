using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Autor:EntidadBase
{
    public int CodigoAutor { get; set; }
    public string NombreAutor { get; set; } = null!;
    public string? PaisOrigen { get; set; }
    public int CodigoIdioma { get; set; }

    public Idioma? Idioma { get; set; }
    public ICollection<LibroAutor>? LibrosAutores { get; set; }
}