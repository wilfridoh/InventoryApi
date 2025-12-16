using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using InventoryApi.Infraestructura.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApi.Infraestructura.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync(int? productoId = null, DateTime? fromDate = null, DateTime? toDate = null, string? tipo = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductoId", productoId ?? (object)DBNull.Value),
                new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value),
                new SqlParameter("@Tipo", tipo ?? (object)DBNull.Value)
            };

            return await _context.Transacciones.FromSqlRaw("EXEC dbo.sp_ObtenerTransacciones @ProductoId, @FromDate, @ToDate, @Tipo", parameters).ToListAsync();
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            var parameters = new[]
            {
                new SqlParameter("@Tipo", transaction.Tipo ?? (object)DBNull.Value),
                new SqlParameter("@ProductoId", transaction.ProductoId),
                new SqlParameter("@Cantidad", transaction.Cantidad),
                new SqlParameter("@PrecioUnitario", transaction.PrecioUnitario),
                new SqlParameter("@Detalle", transaction.Detalle ?? (object)DBNull.Value)
            };

            try
            {
                // Ejecuta el SP que inserta transacción y ajusta stock dentro de la base
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.sp_InsertarTransaccion @Tipo, @ProductoId, @Cantidad, @PrecioUnitario, @Detalle", parameters);
            }
            catch (SqlException sqlEx)
            {
                // Capturamos y re-lanzamos información útil del error SQL
                throw new InvalidOperationException($"Error al ejecutar sp_InsertarTransaccion: Número={sqlEx.Number}, Mensaje={sqlEx.Message}", sqlEx);
            }
            catch (DbException dbEx)
            {
                throw new InvalidOperationException($"Error de base de datos al ejecutar sp_InsertarTransaccion: {dbEx.Message}", dbEx);
            }

            // Recupera datos actualizados del producto (nombre y stock)
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductoId == transaction.ProductoId);

            transaction.ProductoNombre = producto?.Nombre;
            transaction.StockActual = producto?.Stock;

            // Recupera la transacción insertada de forma heurística (mejor que por fecha)

            var inserted = await _context.Transacciones
                .AsNoTracking()
                .Where(t => t.ProductoId == transaction.ProductoId && t.Cantidad == transaction.Cantidad && t.PrecioUnitario == transaction.PrecioUnitario)
                .OrderByDescending(t => t.TransaccionId)
                .FirstOrDefaultAsync();

            if (inserted is not null)
            {
                transaction.TransaccionId = inserted.TransaccionId;
                transaction.Fecha = inserted.Fecha;
            }

            return transaction;
        }
    }
}
