using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class TandaLabor:EntidadBase
{
    public int CodigoTanda { get; set; }
    public string NombreTanda { get; set; } = null!;
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }

    public ICollection<Empleado>? Empleados { get; set; }
}