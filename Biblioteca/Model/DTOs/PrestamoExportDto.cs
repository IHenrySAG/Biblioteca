using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model.DTOs;
public class PrestamoExportDto
{
    public int CodigoPrestamo { get; set; }
    public string Libro { get; set; }
    public string Estudiante { get; set; }
    public string Empleado { get; set; }
    public string? FechaPrestamo { get; set; }
    public string? FechaDevolucionEsperada { get; set; }
    public string? FechaDevolucion { get; set; }
    public decimal? MontoDia { get; set; }
    public decimal? MontoDiaRetraso { get; set; }
    public decimal? MontoTotal { get; set; }
}