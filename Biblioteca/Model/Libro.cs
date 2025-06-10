using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Libro : EntidadBase
{
    public int CodigoLibro { get; set; }
    public string NombreAutor { get; set; } = null!;
    public string? PaisOrigen { get; set; }
    public int CodigoIdioma { get; set; }

    // Propiedad de navegación
    public Idioma? Idioma { get; set; } = null!;
}