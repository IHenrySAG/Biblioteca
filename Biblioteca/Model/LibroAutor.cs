using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class LibroAutor
{
    public int Id { get; set; }
    public int CodigoLibro { get; set; }
    public int CodigoAutor { get; set; }

    public Libro? Libro { get; set; }
    public Autor? Autor { get; set; }
}