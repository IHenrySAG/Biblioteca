namespace Biblioteca.Model.ViewModel;

public class LibroVM
{
    public int CodigoLibro { get; set; }
    public string? Titulo { get; set; }
    public string? SignaturaTopografica { get; set; }
    public string? Isbn { get; set; }
    public string Editora { get; set; }
    public int AnioPublicacion { get; set; }
    public string? Ciencia { get; set; }
    public string? TipoBibliografia { get; set; }
    public string? Idioma { get; set; }
    public int Inventario { get; set; }
    public IEnumerable<string> Autores { get; set; }
}
