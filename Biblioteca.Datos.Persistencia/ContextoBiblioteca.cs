using Biblioteca.Datos.Persistencia.Dominio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Datos.Persistencia;
public class ContextoBiblioteca: DbContext
{
    public ContextoBiblioteca(DbContextOptions<ContextoBiblioteca> options) : base(options)
    {
    }
    public DbSet<TipoBibliografia> TiposBibliografias { get; set; }
    public DbSet<Editora> Editoras { get; set; }
    public DbSet<Idioma> Idiomas { get; set; }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoBibliografia>(builder =>
        {
            builder.ToTable("TIPOS_BIBLIOGRAFIAS");

            builder.HasKey(t => t.CodigoBibliografia);

            builder.Property(t => t.CodigoBibliografia)
                .HasColumnName("CODIGO_BIBLIOGRAFIA");

            builder.Property(t => t.NombreBibliografia)
                .HasColumnName("NOMBRE_BIBLIOGRAFIA")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(t => t.Descripcion)
                .HasColumnName("DESCRIPCION");

            builder.Property(t => t.Estado)
                .HasColumnName("ESTADO");
        });

        // Configuración de la tabla EDITORAS
        modelBuilder.Entity<Editora>(builder =>
        {
            builder.ToTable("EDITORAS");

            builder.HasKey(e => e.CodigoEditora);

            builder.Property(e => e.CodigoEditora)
                .HasColumnName("CODIGO_EDITORA");

            builder.Property(e => e.NombreEditora)
                .HasColumnName("NOMBRE_EDITORA")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(e => e.Descripcion)
                .HasColumnName("DESCRIPCION");

            builder.Property(e => e.Estado)
                .HasColumnName("ESTADO");
        });

        // Configuración de la tabla IDIOMAS
        modelBuilder.Entity<Idioma>(builder =>
        {
            builder.ToTable("IDIOMAS");

            builder.HasKey(i => i.CodigoIdioma);

            builder.Property(i => i.CodigoIdioma)
                .HasColumnName("CODIGO_IDIOMA");

            builder.Property(i => i.NombreIdioma)
                .HasColumnName("NOMBRE_IDIOMA")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.Estado)
                .HasColumnName("ESTADO");
        });

        // Configuración de la tabla AUTORES
        modelBuilder.Entity<Autor>(builder =>
        {
            builder.ToTable("AUTORES");

            builder.HasKey(a => a.CodigoAutor);

            builder.Property(a => a.CodigoAutor)
                .HasColumnName("CODIGO_AUTOR");

            builder.Property(a => a.NombreAutor)
                .HasColumnName("NOMBRE_AUTOR")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(a => a.PaisOrigen)
                .HasColumnName("PAIS_ORIGEN")
                .HasMaxLength(50);

            builder.Property(a => a.CodigoIdioma)
                .HasColumnName("CODIGO_IDIOMA")
                .IsRequired();

            builder.Property(a => a.Estado)
                .HasColumnName("ESTADO");

            builder.HasOne(a => a.Idioma)
                .WithMany(i => i.Autores)
                .HasForeignKey(a => a.CodigoIdioma)
                .HasConstraintName("FK_IDIOMA_AUTOR");
        });

        // Configuración de la tabla LIBROS
        modelBuilder.Entity<Libro>(builder =>
        {
            builder.ToTable("LIBROS");

            builder.HasKey(l => l.CodigoLibro);

            builder.Property(l => l.CodigoLibro)
                .HasColumnName("CODIGO_LIBRO");

            builder.Property(l => l.NombreAutor)
                .HasColumnName("NOMBRE_AUTOR")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(l => l.PaisOrigen)
                .HasColumnName("PAIS_ORIGEN")
                .HasMaxLength(50);

            builder.Property(l => l.CodigoIdioma)
                .HasColumnName("CODIGO_IDIOMA")
                .IsRequired();

            builder.Property(l => l.Estado)
                .HasColumnName("ESTADO");

            builder.HasOne(l => l.Idioma)
                .WithMany(i => i.Libros)
                .HasForeignKey(l => l.CodigoIdioma)
                .HasConstraintName("FK_IDIOMA_LIBRO");
        });

        base.OnModelCreating(modelBuilder);
    }
}