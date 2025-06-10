
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Model;

namespace Biblioteca;
public class ContextoBiblioteca: DbContext
{
    public ContextoBiblioteca(DbContextOptions<ContextoBiblioteca> options) : base(options)
    {
    }

    public DbSet<Autor> Autores { get; set; }
    public DbSet<Editora> Editoras { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Idioma> Idiomas { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<LibroAutor> LibrosAutores { get; set; }
    public DbSet<LibroBibliografia> LibrosBibliografias { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<TandaLabor> TandasLabor { get; set; }
    public DbSet<TipoBibliografia> TiposBibliografias { get; set; }
    public DbSet<TipoPersona> TiposPersonas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TIPOS_BIBLIOGRAFIAS
        modelBuilder.Entity<TipoBibliografia>(builder =>
        {
            builder.ToTable("TIPOS_BIBLIOGRAFIAS");
            builder.HasKey(t => t.CodigoBibliografia);
            builder.Property(t => t.CodigoBibliografia).HasColumnName("CODIGO_BIBLIOGRAFIA");
            builder.Property(t => t.NombreBibliografia).HasColumnName("NOMBRE_BIBLIOGRAFIA").HasMaxLength(30).IsRequired();
            builder.Property(t => t.Descripcion).HasColumnName("DESCRIPCION");
            builder.Property(t => t.Estado).HasColumnName("ESTADO");
        });

        // EDITORAS
        modelBuilder.Entity<Editora>(builder =>
        {
            builder.ToTable("EDITORAS");
            builder.HasKey(e => e.CodigoEditora);
            builder.Property(e => e.CodigoEditora).HasColumnName("CODIGO_EDITORA");
            builder.Property(e => e.NombreEditora).HasColumnName("NOMBRE_EDITORA").HasMaxLength(30).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("DESCRIPCION");
            builder.Property(e => e.Estado).HasColumnName("ESTADO");
        });

        // IDIOMAS
        modelBuilder.Entity<Idioma>(builder =>
        {
            builder.ToTable("IDIOMAS");
            builder.HasKey(i => i.CodigoIdioma);
            builder.Property(i => i.CodigoIdioma).HasColumnName("CODIGO_IDIOMA");
            builder.Property(i => i.NombreIdioma).HasColumnName("NOMBRE_IDIOMA").HasMaxLength(15).IsRequired();
            builder.Property(i => i.Estado).HasColumnName("ESTADO");
        });

        // AUTORES
        modelBuilder.Entity<Autor>(builder =>
        {
            builder.ToTable("AUTORES");
            builder.HasKey(a => a.CodigoAutor);
            builder.Property(a => a.CodigoAutor).HasColumnName("CODIGO_AUTOR");
            builder.Property(a => a.NombreAutor).HasColumnName("NOMBRE_AUTOR").HasMaxLength(60).IsRequired();
            builder.Property(a => a.PaisOrigen).HasColumnName("PAIS_ORIGEN").HasMaxLength(50);
            builder.Property(a => a.CodigoIdioma).HasColumnName("CODIGO_IDIOMA").IsRequired();
            builder.Property(a => a.Estado).HasColumnName("ESTADO");
            builder.HasOne(a => a.Idioma)
                .WithMany(i => i.Autores)
                .HasForeignKey(a => a.CodigoIdioma)
                .HasConstraintName("FK_IDIOMA_AUTOR");
        });

        // LIBROS
        modelBuilder.Entity<Libro>(builder =>
        {
            builder.ToTable("LIBROS");
            builder.HasKey(l => l.CodigoLibro);
            builder.Property(l => l.CodigoLibro).HasColumnName("CODIGO_LIBRO");
            builder.Property(l => l.Titulo).HasColumnName("TITULO").HasMaxLength(100);
            builder.Property(l => l.SignaturaTopografica).HasColumnName("SIGNATURA_TOPOGRAFICA").HasMaxLength(50);
            builder.Property(l => l.Isbn).HasColumnName("ISBN").HasMaxLength(15);
            builder.Property(l => l.CodigoEditora).HasColumnName("CODIGO_EDITORA").IsRequired();
            builder.Property(l => l.AnioPublicacion).HasColumnName("ANIO_PUBLICACION").IsRequired();
            builder.Property(l => l.Ciencia).HasColumnName("CIENCIA").HasMaxLength(30);
            builder.Property(l => l.CodigoIdioma).HasColumnName("CODIGO_IDIOMA").IsRequired();
            builder.Property(l => l.Estado).HasColumnName("ESTADO");
            builder.HasOne(l => l.Editora)
                .WithMany(e => e.Libros)
                .HasForeignKey(l => l.CodigoEditora)
                .HasConstraintName("FK_EDITORA_LIBROS");
            builder.HasOne(l => l.Idioma)
                .WithMany(i => i.Libros)
                .HasForeignKey(l => l.CodigoIdioma)
                .HasConstraintName("FK_IDIOMA_LIBROS");
        });

        // LIBROS_BIBLIOGRAFIAS (Many-to-Many)
        modelBuilder.Entity<LibroBibliografia>(builder =>
        {
            builder.ToTable("LIBROS_BIBLIOGRAFIAS");
            builder.HasKey(lb => lb.Id);
            builder.Property(lb => lb.Id).HasColumnName("ID");
            builder.Property(lb => lb.CodigoLibro).HasColumnName("CODIGO_LIBRO").IsRequired();
            builder.Property(lb => lb.CodigoBibliografia).HasColumnName("CODIGO_BIBLIOGRAFIA").IsRequired();
            builder.HasOne(lb => lb.Libro)
                .WithMany(l => l.LibrosBibliografias)
                .HasForeignKey(lb => lb.CodigoLibro)
                .HasConstraintName("FK_LIBROS_BIBLIOGRAFIAS_LIBRO");
            builder.HasOne(lb => lb.TipoBibliografia)
                .WithMany(tb => tb.LibrosBibliografias)
                .HasForeignKey(lb => lb.CodigoBibliografia)
                .HasConstraintName("FK_LIBROS_BIBLIOGRAFIAS_BIBLIOGRAFIA");
        });

        // LIBROS_AUTORES (Many-to-Many)
        modelBuilder.Entity<LibroAutor>(builder =>
        {
            builder.ToTable("LIBROS_AUTORES");
            builder.HasKey(la => la.Id);
            builder.Property(la => la.Id).HasColumnName("ID");
            builder.Property(la => la.CodigoLibro).HasColumnName("CODIGO_LIBRO").IsRequired();
            builder.Property(la => la.CodigoAutor).HasColumnName("CODIGO_AUTOR").IsRequired();
            builder.HasOne(la => la.Libro)
                .WithMany(l => l.LibrosAutores)
                .HasForeignKey(la => la.CodigoLibro)
                .HasConstraintName("FK_LIBROS_AUTORES_LIBRO");
            builder.HasOne(la => la.Autor)
                .WithMany(a => a.LibrosAutores)
                .HasForeignKey(la => la.CodigoAutor)
                .HasConstraintName("FK_LIBROS_AUTORES_AUTOR");
        });

        // PRESTAMO
        modelBuilder.Entity<Prestamo>(builder =>
        {
            builder.ToTable("PRESTAMO");
            builder.HasKey(p => p.CodigoPrestamo);
            builder.Property(p => p.CodigoPrestamo).HasColumnName("CODIGO_PRESTAMO");
            builder.Property(p => p.CodigoEmpleado).HasColumnName("CODIGO_EMPLEADO").IsRequired();
            builder.Property(p => p.CodigoLibro).HasColumnName("CODIGO_LIBRO").IsRequired();
            builder.Property(p => p.CodigoUsuario).HasColumnName("CODIGO_USUARIO").IsRequired();
            builder.Property(p => p.FechaPrestamo).HasColumnName("FECHA_PRESTAMO").IsRequired();
            builder.Property(p => p.FechaDevolucion).HasColumnName("FECHA_DEVOLUCION");
            builder.Property(p => p.MontoDia).HasColumnName("MONTO_DIA").HasColumnType("decimal(10,2)");
            builder.Property(p => p.CantidadDias).HasColumnName("CANTIDAD_DIAS");
            builder.Property(p => p.Comentario).HasColumnName("COMENTARIO");
            builder.Property(p => p.Estado).HasColumnName("ESTADO");
            builder.HasOne(p => p.Empleado)
                .WithMany()
                .HasForeignKey(p => p.CodigoEmpleado)
                .HasConstraintName("FK_EMPLEADO_PRESTAMO");
            builder.HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.CodigoLibro)
                .HasConstraintName("FK_LIBRO_PRESTAMO");
            builder.HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.CodigoUsuario)
                .HasConstraintName("FK_USUARIO_PRESTAMO");
        });

        base.OnModelCreating(modelBuilder);
    }
}