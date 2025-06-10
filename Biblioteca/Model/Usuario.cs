using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Usuario:EntidadBase
{
    public int CodigoUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Cedula { get; set; } = null!;
    public string NumeroCarnet { get; set; } = null!;
    public int CodigoTipo { get; set; }

    public TipoPersona? TipoPersona { get; set; }
    public ICollection<Prestamo>? Prestamos { get; set; }
}