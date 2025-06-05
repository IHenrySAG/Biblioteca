using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Persistencia.Dominio;
public class TipoBibliografia : EntidadBase
{
    public int CodigoBibliografia { get; set; }
    public string NombreBibliografia { get; set; } = null!;
    public string? Descripcion { get; set; }
}