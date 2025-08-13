using Newtonsoft.Json.Linq;

namespace Biblioteca.Common;

public static class Extensions
{
    public static string ToCedulaFormat(this string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 11)
            return cedula;

        return $"{cedula.Substring(0, 3)}-{cedula.Substring(3, 7)}-{cedula.Substring(10, 1)}";
    }

    public static string ToMaxLength(this string? texto, int maxLenght)
    {
        if (string.IsNullOrWhiteSpace(texto) || texto.Length <= maxLenght)
            return texto;

        return texto.Substring(0, maxLenght - 3) + "...";
    }
}
