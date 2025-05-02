using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace blackcat.Models;

public partial class BlackcatDbContext : DbContext
{
    public BlackcatDbContext()
    {
    }

    public BlackcatDbContext(DbContextOptions<BlackcatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Busquedum> Busqueda { get; set; }

    public virtual DbSet<EstadoUsulibro> EstadoUsulibros { get; set; }

    public virtual DbSet<Informacion> Informacions { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<ListaU> ListaUs { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Tipoinfo> Tipoinfos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=blackcatDB;User Id=Shirly;Password=230425sd@;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Busquedum>(entity =>
        {
            entity.HasKey(e => e.IdBus).HasName("PK__busqueda__383854C874BC6B66");

            entity.ToTable("busqueda");

            entity.Property(e => e.IdBus).HasColumnName("idBus");
            entity.Property(e => e.CantB).HasColumnName("cantB");
            entity.Property(e => e.CantBn).HasColumnName("cantBN");
            entity.Property(e => e.IdEstadoBus).HasColumnName("id_estadoBus");
            entity.Property(e => e.IdLibro).HasColumnName("id_libro");
            entity.Property(e => e.NomLib)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("nomLib");

            entity.HasOne(d => d.IdEstadoBusNavigation).WithMany(p => p.Busqueda)
                .HasForeignKey(d => d.IdEstadoBus)
                .HasConstraintName("FK__busqueda__id_est__09A971A2");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.Busqueda)
                .HasForeignKey(d => d.IdLibro)
                .HasConstraintName("FK__busqueda__id_lib__0A9D95DB");
        });

        modelBuilder.Entity<EstadoUsulibro>(entity =>
        {
            entity.HasKey(e => e.IdE).HasName("PK__estadoUs__DC501A2B4AF2F25C");

            entity.ToTable("estadoUsulibro");

            entity.Property(e => e.IdE).HasColumnName("idE");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("estado");
        });

        modelBuilder.Entity<Informacion>(entity =>
        {
            entity.HasKey(e => e.IdInfo).HasName("PK__informac__B0BF47D71C7A20DC");

            entity.ToTable("informacion");

            entity.Property(e => e.IdInfo).HasColumnName("idInfo");
            entity.Property(e => e.Descrip)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("descrip");
            entity.Property(e => e.FechaI)
                .HasColumnType("datetime")
                .HasColumnName("fechaI");
            entity.Property(e => e.IdTipoinfo).HasColumnName("id_tipoinfo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdTipoinfoNavigation).WithMany(p => p.Informacions)
                .HasForeignKey(d => d.IdTipoinfo)
                .HasConstraintName("FK__informaci__id_ti__05D8E0BE");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Informacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__informaci__id_us__04E4BC85");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.IdL).HasName("PK__libro__DC501A2413CA483C");

            entity.ToTable("libro");

            entity.Property(e => e.IdL).HasColumnName("idL");
            entity.Property(e => e.Archivo)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("archivo");
            entity.Property(e => e.Autor)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("autor");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Imagen)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("imagen");
            entity.Property(e => e.NombreL)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("nombreL");
        });

        modelBuilder.Entity<ListaU>(entity =>
        {
            entity.HasKey(e => e.IdLista).HasName("PK__listaU__6C8A0FE50F278F10");

            entity.ToTable("listaU");

            entity.Property(e => e.IdLista).HasColumnName("idLista");
            entity.Property(e => e.IdLibro).HasColumnName("id_libro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.ListaUs)
                .HasForeignKey(d => d.IdLibro)
                .HasConstraintName("FK__listaU__id_libro__00200768");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ListaUs)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__listaU__id_usuar__7F2BE32F");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Idrol).HasName("PK__rol__24C6BB20A5C74BCD");

            entity.ToTable("rol");

            entity.Property(e => e.Idrol).HasColumnName("idrol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipoinfo>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK__tipoinfo__BDD0DFE1E3F546F2");

            entity.ToTable("tipoinfo");

            entity.Property(e => e.IdTipo).HasColumnName("idTipo");
            entity.Property(e => e.Tipo).HasColumnName("tipo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdU).HasName("PK__usuario__DC501A1BA9114706");

            entity.ToTable("usuario");

            entity.Property(e => e.IdU).HasColumnName("idU");
            entity.Property(e => e.Cont)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("cont");
            entity.Property(e => e.CorreoU)
                .HasMaxLength(100)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("correoU");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreU)
                .HasMaxLength(50)
                .IsUnicode(false)
                .UseCollation("Modern_Spanish_CI_AS")
                .HasColumnName("nombreU");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("FK__usuario__id_esta__778AC167");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__usuario__id_rol__76969D2E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
