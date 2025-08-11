
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
    public DbSet<Rol> Roles { get; set; }
    public DbSet<TandaLabor> TandasLabor { get; set; }
    public DbSet<TipoBibliografia> TiposBibliografias { get; set; }
    public DbSet<TipoPersona> TiposPersonas { get; set; }
    public DbSet<Estudiante> Estudiantes { get; set; }

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
            builder.Property(t => t.Eliminado).HasColumnName("ELIMINADO");
        });

        // EDITORAS
        modelBuilder.Entity<Editora>(builder =>
        {
            builder.ToTable("EDITORAS");
            builder.HasKey(e => e.CodigoEditora);
            builder.Property(e => e.CodigoEditora).HasColumnName("CODIGO_EDITORA");
            builder.Property(e => e.NombreEditora).HasColumnName("NOMBRE_EDITORA").HasMaxLength(30).IsRequired();
            builder.Property(e => e.Descripcion).HasColumnName("DESCRIPCION");
            builder.Property(e => e.Eliminado).HasColumnName("ELIMINADO");
        });

        // IDIOMAS
        modelBuilder.Entity<Idioma>(builder =>
        {
            builder.ToTable("IDIOMAS");
            builder.HasKey(i => i.CodigoIdioma);
            builder.Property(i => i.CodigoIdioma).HasColumnName("CODIGO_IDIOMA");
            builder.Property(i => i.NombreIdioma).HasColumnName("NOMBRE_IDIOMA").HasMaxLength(15).IsRequired();
            builder.Property(i => i.Eliminado).HasColumnName("ELIMINADO");
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
            builder.Property(a => a.Eliminado).HasColumnName("ELIMINADO");
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
            builder.Property(l => l.Inventario).HasColumnName("INVENTARIO").IsRequired();
            builder.Property(l => l.Eliminado).HasColumnName("ELIMINADO");
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
            builder.Property(p => p.CodigoEstudiante).HasColumnName("CODIGO_ESTUDIANTE").IsRequired();
            builder.Property(p => p.FechaPrestamo).HasColumnName("FECHA_PRESTAMO").IsRequired();
            builder.Property(p => p.FechaDevolucionEsperada).HasColumnName("FECHA_DEVOLUCION_ESPERADA");
            builder.Property(p => p.FechaDevolucion).HasColumnName("FECHA_DEVOLUCION");
            builder.Property(p => p.MontoDia).HasColumnName("MONTO_DIA").HasColumnType("decimal(10,2)");
            builder.Property(p => p.MontoDiaRetraso).HasColumnName("MONTO_DIA_RETRASO").HasColumnType("decimal(10,2)");
            builder.Property(p => p.MontoTotal).HasColumnName("MONTO_TOTAL").HasColumnType("decimal(10,2)");
            builder.Property(p => p.Comentario).HasColumnName("COMENTARIO");
            builder.Property(p => p.Eliminado).HasColumnName("ELIMINADO");
            builder.HasOne(p => p.Empleado)
                .WithMany(e=>e.Prestamos)
                .HasForeignKey(p => p.CodigoEmpleado)
                .HasConstraintName("FK_EMPLEADO_PRESTAMO");
            builder.HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.CodigoLibro)
                .HasConstraintName("FK_LIBRO_PRESTAMO");
            builder.HasOne(p => p.Estudiante)
                .WithMany(e=>e.Prestamos)
                .HasForeignKey(p => p.CodigoEstudiante)
                .HasConstraintName("FK_ESTUDIANTE_PRESTAMO");
        });

        // ROLES
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("ROLES");
            entity.HasKey(e => e.CodigoRol).HasName("PK_ROLES");
            entity.Property(e => e.CodigoRol).HasColumnName("CODIGO_ROL").ValueGeneratedOnAdd();
            entity.Property(e => e.NombreRol).HasColumnName("NOMBRE_ROL").IsRequired().HasMaxLength(15);
            entity.Property(e => e.Eliminado).HasColumnName("ELIMINADO").IsRequired(false);
        });

        // EMPLEADOS
        modelBuilder.Entity<Empleado>(builder =>
        {
            builder.ToTable("EMPLEADOS");
            builder.Property(e => e.CodigoEmpleado).HasColumnName("CODIGO_EMPLEADO");
            builder.Property(e => e.Nombre).HasColumnName("NOMBRE").HasMaxLength(80).IsRequired();
            builder.Property(e => e.Apellido).HasColumnName("APELLIDO").HasMaxLength(80).IsRequired();
            builder.Property(e => e.Cedula).HasColumnName("CEDULA").HasMaxLength(11).IsRequired();
            builder.Property(e => e.CodigoTanda).HasColumnName("CODIGO_TANDA").IsRequired();
            builder.Property(e => e.PorcentajeComision).HasColumnName("PORCENTAJE_COMISION");
            builder.Property(e => e.FechaIngreso).HasColumnName("FECHA_INGRESO");
            builder.Property(e => e.NombreUsuario).HasColumnName("NOMBRE_USUARIO");
            builder.Property(e => e.Contrasenia).HasColumnName("CONTRASENIA");
            builder.Property(e => e.CodigoRol).HasColumnName("CODIGO_ROL");
            builder.Property(e => e.Eliminado).HasColumnName("ELIMINADO");
            builder.HasKey(e => e.CodigoEmpleado).HasName("CODIGO_EMPLEADO");

            builder.HasOne(e => e.TandaLabor)
                .WithMany(t => t.Empleados)
                .HasForeignKey(e => e.CodigoTanda)
                .HasConstraintName("FK_TANDA_EMPLEADO");

            builder.HasOne(e => e.Rol)
                .WithMany(r => r.Empleados)
                .HasForeignKey(e => e.CodigoRol)
                .HasConstraintName("FK_ROL_EMPLEADO");

            builder.HasMany(e => e.Prestamos)
                .WithOne(p => p.Empleado)
                .HasForeignKey(p => p.CodigoEmpleado)
                .HasConstraintName("FK_EMPLEADO_PRESTAMO");
        });

        // TANDA_LABOR
        modelBuilder.Entity<TandaLabor>(builder =>
        {
            builder.ToTable("TANDA_LABOR");
            builder.HasKey(t => t.CodigoTanda);
            builder.Property(t => t.CodigoTanda).HasColumnName("CODIGO_TANDA");
            builder.Property(t => t.NombreTanda).HasColumnName("NOMBRE_TANDA").HasMaxLength(50); // Ajusta el tamaño si es necesario
            builder.Property(t => t.HoraInicio).HasColumnName("HORA_INICIO").IsRequired();
            builder.Property(t => t.HoraFin).HasColumnName("HORA_FIN").IsRequired();
            builder.Property(t => t.Eliminado).HasColumnName("ELIMINADO");
        });

        // TIPO_PERSONAS
        modelBuilder.Entity<TipoPersona>(builder =>
        {
            builder.ToTable("TIPOS_PERSONAS");
            builder.HasKey(tp => tp.CodigoTipo);
            builder.Property(tp => tp.CodigoTipo).HasColumnName("CODIGO_TIPO");
            builder.Property(tp => tp.NombreTipo).HasColumnName("NOMBRE_TIPO").HasMaxLength(8).IsRequired();
            builder.Property(tp => tp.Eliminado).HasColumnName("ELIMINADO");
        });

        // ESTUDIANTES
        modelBuilder.Entity<Estudiante>(builder =>
        {
            builder.ToTable("ESTUDIANTES");
            builder.HasKey(u => u.CodigoEstudiante);
            builder.Property(u => u.CodigoEstudiante).HasColumnName("CODIGO_ESTUDIANTE");
            builder.Property(u => u.Nombre).HasColumnName("NOMBRE").HasMaxLength(80).IsRequired();
            builder.Property(u => u.Apellido).HasColumnName("APELLIDO").HasMaxLength(80).IsRequired();
            builder.Property(u => u.Cedula).HasColumnName("CEDULA").HasMaxLength(11).IsRequired();
            builder.Property(u => u.NumeroCarnet).HasColumnName("NUMERO_CARNET").HasMaxLength(10).IsRequired();
            builder.Property(u => u.CodigoTipo).HasColumnName("CODIGO_TIPO").IsRequired();
            builder.Property(u => u.Eliminado).HasColumnName("ELIMINADO");

            builder.HasOne(u => u.TipoPersona)
                .WithMany(tp => tp.Estudiantes)
                .HasForeignKey(u => u.CodigoTipo)
                .HasConstraintName("FK_TIPO_PERSONA_USUARIO");
        });

        base.OnModelCreating(modelBuilder);
    }
}