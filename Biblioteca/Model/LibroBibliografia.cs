using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class LibroBibliografia
{
    public int Id { get; set; }
    public int CodigoLibro { get; set; }
    public int CodigoBibliografia { get; set; }

    public Libro? Libro { get; set; }
    public TipoBibliografia? TipoBibliografia { get; set; }
}