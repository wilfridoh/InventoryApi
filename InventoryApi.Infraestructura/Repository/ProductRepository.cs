using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using InventoryApi.Infraestructura.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApi.Infraestructura.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? nombre = null, string? categoria = null,
            decimal? minPrice = null, decimal? maxPrice = null, int? minStock = null, int? maxStock = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre ?? (object)DBNull.Value),
                new SqlParameter("@Categoria", categoria ?? (object)DBNull.Value),
                new SqlParameter("@MinPrice", minPrice ?? (object)DBNull.Value),
                new SqlParameter("@MaxPrice", maxPrice ?? (object)DBNull.Value),
                new SqlParameter("@MinStock", minStock ?? (object)DBNull.Value),
                new SqlParameter("@MaxStock", maxStock ?? (object)DBNull.Value)
            };

            return await _context.Productos.FromSqlRaw("EXEC dbo.sp_ObtenerProductos @Nombre, @Categoria, @MinPrice, @MaxPrice, @MinStock, @MaxStock", parameters).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Productos.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Productos.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Productos.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
