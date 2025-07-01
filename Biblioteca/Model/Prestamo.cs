using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Prestamo:EntidadBase
{
    public int CodigoPrestamo { get; set; }
    public int CodigoEmpleado { get; set; }
    public int CodigoLibro { get; set; }
    public int CodigoEstudiante { get; set; }
    public DateOnly FechaPrestamo { get; set; }
    public DateOnly? FechaDevolucion { get; set; }
    public decimal? MontoDia { get; set; }
    public int? CantidadDias { get; set; }
    public string? Comentario { get; set; }

    public Empleado? Empleado { get; set; }
    public Libro? Libro { get; set; }
    public Estudiante? Estudiante { get; set; }
}