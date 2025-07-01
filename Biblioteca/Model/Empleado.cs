using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Empleado:EntidadBase
{
    public int CodigoEmpleado { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;

    [MinLength(11, ErrorMessage = "El tamaño de la cedula es incorrecto")]
    public string Cedula { get; set; } = null!;
    public int CodigoTanda { get; set; }
    public double? PorcentajeComision { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string Contrasenia { get; set; } = null!;

    //Esta propiedad no sera persistida en la base de datos
    [NotMapped]
    public string RepetirContrasenia { get; set; } = null!;
    public int CodigoRol { get; set; }

    public TandaLabor? TandaLabor { get; set; }
    public ICollection<Prestamo>? Prestamos { get; set; }
    public Rol? Rol { get; set; }
}