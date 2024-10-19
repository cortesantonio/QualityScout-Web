using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace ScannerCC.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuarios> Usuario { get; set; }
        public DbSet<Productos> Producto { get; set; }
        public DbSet<Escaneos> Escaneo { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Controles> Controles { get; set; }
        public DbSet<BotellaDetalles> BotellaDetalle { get; set; }
        public DbSet<InformacionQuimica> InformacionQuimica { get; set; }
        public DbSet<Informes> Informe { get; set; }
        public DbSet<ProductoDetalles> ProductoDetalle { get; set; }
        public DbSet<ProductoHistorial> ProductoHistorial { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=qsLocal;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Controles>()
                .HasOne(c => c.Productos)
                .WithMany()
                .HasForeignKey(c => c.IdProductos)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            modelBuilder.Entity<Controles>()
                .HasOne(c => c.Usuarios)
                .WithMany()
                .HasForeignKey(c => c.IdUsuarios)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            modelBuilder.Entity<Escaneos>()
                .HasOne(e => e.Productos)
                .WithMany()
                .HasForeignKey(e => e.IdProductos)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            modelBuilder.Entity<Escaneos>()
                .HasOne(e => e.Usuarios)
                .WithMany()
                .HasForeignKey(e => e.IdUsuarios)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict

            modelBuilder.Entity<Controles>()
                .HasOne(c => c.Productos)
                .WithMany()
                .HasForeignKey(c => c.IdProductos)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            modelBuilder.Entity<Controles>()
                .HasOne(c => c.Usuarios)
                .WithMany()
                .HasForeignKey(c => c.IdUsuarios)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            modelBuilder.Entity<ProductoDetalles>()
                .HasOne(pd => pd.Productos)
                .WithMany()
                .HasForeignKey(pd => pd.IdProductos)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            modelBuilder.Entity<ProductoDetalles>()
                .HasOne(pd => pd.BotellaDetalles)
                .WithMany()
                .HasForeignKey(pd => pd.IdBotellaDetalles)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado de Cascade a Restrict

            base.OnModelCreating(modelBuilder);
        }


    }
}