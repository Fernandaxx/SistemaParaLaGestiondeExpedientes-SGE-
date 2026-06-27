using Microsoft.EntityFrameworkCore;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Datos;

public class SgeContext : DbContext
{
    public SgeContext(DbContextOptions<SgeContext> options) : base(options)
    {
    }

    public DbSet<Expediente> Expedientes => Set<Expediente>();
    public DbSet<Tramite> Tramites => Set<Tramite>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.ToTable("Expedientes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.UsuarioUltimoCambio).IsRequired();
            entity.Property(e => e.Estado).HasConversion<string>().IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.Property(e => e.FechaModificacion).IsRequired();

            entity.ComplexProperty(e => e.Caratula, caratula =>
            {
                caratula.Property(c => c.Valor)
                    .HasColumnName("Caratula")
                    .IsRequired();
            });
        });

        modelBuilder.Entity<Tramite>(entity =>
        {
            entity.ToTable("Tramites");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id).ValueGeneratedNever();
            entity.Property(t => t.ExpedienteId).IsRequired();
            entity.Property(t => t.UsuarioUltimoCambio).IsRequired();
            entity.Property(t => t.Etiqueta).HasConversion<string>().IsRequired();
            entity.Property(t => t.FechaCreacion).IsRequired();
            entity.Property(t => t.FechaUltimaModificacion).IsRequired();

            entity.ComplexProperty(t => t.Contenido, contenido =>
            {
                contenido.Property(c => c.Valor)
                    .HasColumnName("Contenido")
                    .IsRequired();
            });

            entity.HasOne<Expediente>()
                .WithMany()
                .HasForeignKey(t => t.ExpedienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id).ValueGeneratedNever();
            entity.Property(u => u.Nombre).IsRequired();
            entity.Property(u => u.CorreoElectronico).IsRequired();
            entity.Property(u => u.ContrasenaHash).IsRequired();
            entity.Property(u => u.EsAdministrador).IsRequired();
            entity.HasIndex(u => u.CorreoElectronico).IsUnique();

            entity.Ignore(u => u.Permisos);
        });

        modelBuilder.SharedTypeEntity<Dictionary<string, object>>("UsuarioPermisos", entity =>
        {
            entity.ToTable("UsuarioPermisos");
            entity.IndexerProperty<Guid>("UsuarioId").IsRequired();
            entity.IndexerProperty<Permiso>("Permiso").HasConversion<string>().IsRequired();
            entity.HasKey("UsuarioId", "Permiso");

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey("UsuarioId")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
