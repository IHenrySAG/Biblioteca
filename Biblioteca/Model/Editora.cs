using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Editora:EntidadBase
{
    public int CodigoEditora { get; set; }
    public string NombreEditora { get; set; } = null!;
    public string? Descripcion { get; set; }
}