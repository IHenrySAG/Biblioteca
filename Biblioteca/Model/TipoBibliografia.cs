using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class TipoBibliografia : EntidadBase
{
    public int CodigoBibliografia { get; set; }
    public string NombreBibliografia { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<LibroBibliografia> LibrosBibliografias { get; set; } = [];
}