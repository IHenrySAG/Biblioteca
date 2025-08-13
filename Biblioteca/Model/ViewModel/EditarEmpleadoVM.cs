using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Model.ViewModel;

public class EditarEmpleadoVM
{
    public int CodigoEmpleado { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;

    [MinLength(11, ErrorMessage = "El tamaño de la cedula es incorrecto")]
    public string Cedula { get; set; } = null!;
    public int CodigoTanda { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public int CodigoRol { get; set; }
}
