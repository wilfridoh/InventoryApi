using InventoryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApi.Infraestructura.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Productos { get; set; } = null!;
        public DbSet<Transaction> Transacciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Productos").HasKey(p => p.ProductoId);
            modelBuilder.Entity<Transaction>().ToTable("Transacciones").HasKey(t => t.TransaccionId);

            // NOTA: ProductoNombre y StockActual son columnas calculadas devueltas por el SP,
            // no existen físicamente en la tabla Transacciones. No mapearlas evita que EF
            // genere consultas que busquen esas columnas en la tabla.
        }
    }
}
