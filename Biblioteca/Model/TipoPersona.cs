using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;

public class TipoPersona:EntidadBase
{
    public int CodigoTipo { get; set; }
    public string NombreTipo { get; set; } = null!;

    public ICollection<Estudiante> Estudiantes { get; set; }
}