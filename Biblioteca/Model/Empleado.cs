using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Empleado:EntidadBase
{
    public int CodigoEmpleado { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Cedula { get; set; } = null!;
    public int CodigoTanda { get; set; }
    public double? PorcentajeComision { get; set; }
    public DateOnly? FechaIngreso { get; set; }

    public TandaLabor? TandaLabor { get; set; }
    public ICollection<Prestamo>? Prestamos { get; set; }
}