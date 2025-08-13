using Biblioteca.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model.ViewModel;
public class EstudianteVM
{
    public int CodigoEstudiante { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Cedula { get; set; } = null!;
    public string NumeroCarnet { get; set; } = null!;
    public int CodigoTipo { get; set; }
    public bool PuedeTomarPrestamos { get; set; }
    public string ErrorPrestamo { get; set; }

}