using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Models;

namespace sistemaapuestas.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Billetera> Billeteras => Set<Billetera>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Partido> Partidos => Set<Partido>();
    public DbSet<Cuota> Cuotas => Set<Cuota>();
    public DbSet<Apuesta> Apuestas => Set<Apuesta>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasIndex(u => u.Correo).IsUnique();
            e.Property(u => u.Estado).HasDefaultValue("activo");
            e.Property(u => u.FechaRegistro).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Billetera>(e =>
        {
            e.HasIndex(b => b.UsuarioId).IsUnique();
            e.Property(b => b.Saldo).HasDefaultValue(0.00m);
            e.Property(b => b.Moneda).HasDefaultValue("USD");
            e.HasOne(b => b.Usuario)
                .WithOne(u => u.Billetera)
                .HasForeignKey<Billetera>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Movimiento>(e =>
        {
            e.Property(m => m.Tipo)
                .HasConversion<string>();
            e.Property(m => m.Fecha).HasDefaultValueSql("GETDATE()");
            e.HasOne(m => m.Billetera)
                .WithMany(b => b.Movimientos)
                .HasForeignKey(m => m.BilleteraId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Partido>(e =>
        {
            e.Property(p => p.Estado).HasDefaultValue("programado");
            e.Property(p => p.ApuestasHabilitadas).HasDefaultValue(true);
            e.HasIndex(p => p.FechaHora);
        });

        modelBuilder.Entity<Cuota>(e =>
        {
            e.Property(c => c.FechaActualizacion).HasDefaultValueSql("GETDATE()");
            e.HasOne(c => c.Partido)
                .WithMany(p => p.Cuotas)
                .HasForeignKey(c => c.PartidoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Apuesta>(e =>
        {
            e.Property(a => a.Estado).HasDefaultValue("pendiente");
            e.Property(a => a.FechaApuesta).HasDefaultValueSql("GETDATE()");
            e.HasIndex(a => a.UsuarioId);
            e.HasIndex(a => a.PartidoId);
            e.HasOne(a => a.Usuario)
                .WithMany(u => u.Apuestas)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Partido)
                .WithMany(p => p.Apuestas)
                .HasForeignKey(a => a.PartidoId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Notificacion>(e =>
        {
            e.Property(n => n.Leida).HasDefaultValue(false);
            e.Property(n => n.FechaCreacion).HasDefaultValueSql("GETDATE()");
            e.HasIndex(n => n.UsuarioId);
            e.HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
