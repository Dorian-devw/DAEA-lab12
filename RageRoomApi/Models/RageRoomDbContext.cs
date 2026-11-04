using Microsoft.EntityFrameworkCore;

namespace RageRoomApi.Models;

public class RageRoomDbContext : DbContext
{
    public RageRoomDbContext()
    {
    }

    public RageRoomDbContext(DbContextOptions<RageRoomDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Sala> Salas { get; set; } = null!;
    public virtual DbSet<Sesion> Sesiones { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Nombre).IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(250);

            entity.Property(e => e.NivelProteccion)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.PrecioBase)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            entity.Property(e => e.Activa)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<Sesion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ClienteNombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ClienteEmail)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ClienteTelefono)
                .HasMaxLength(20);

            entity.Property(e => e.PrecioTotal)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente")
                .IsRequired();

            entity.HasOne(d => d.Sala)
                .WithMany(p => p.Sesiones)
                .HasForeignKey(d => d.SalaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Sesiones_Salas");
        });
    }
}

