namespace Biblioteca.Views.Empleados;

public class CambiarContrasenia
{
    public int CodigoEmpleado { get; set; }
    public string? ContraseniaActual { get; set; }
    public string NuevaContrasenia { get; set; }
    public string ConfirmarContrasenia { get; set; }
    public string? MensajeError { get; set; }
    // Aquí se pueden agregar métodos para manejar la lógica de cambio de contraseña
}
