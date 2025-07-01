using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Model;
public class Rol:EntidadBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodigoRol { get; set; }

    [Required]
    [StringLength(15)]
    public string NombreRol { get; set; }

    public ICollection<Empleado>? Empleados { get; set; }
}