namespace Biblioteca.Model.DTOs
{
    public class LibroExportDto
    {
        public int CodigoLibro { get; set; }
        public string? Titulo { get; set; }
        public string? SignaturaTopografica { get; set; }
        public string? Isbn { get; set; }
        public int CodigoEditora { get; set; }
        public int AnioPublicacion { get; set; }
        public string? Ciencia { get; set; }
        public int CodigoIdioma { get; set; }
        public bool? Eliminado { get; set; }
    }
}
