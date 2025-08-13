using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model.ViewModel;
public class PrestamoVM
{
    public int CodigoPrestamo { get; set; }
    public string Libro { get; set; }
    public string Estudiante { get; set; }
    public string Empleado { get; set; }
    public DateOnly? FechaPrestamo { get; set; }
    public DateOnly? FechaDevolucionEsperada { get; set; }
    public DateOnly? FechaDevolucion { get; set; }
    public decimal? MontoDia { get; set; }
    public decimal? MontoDiaRetraso { get; set; }
    public decimal? MontoTotal { get; set; }
}