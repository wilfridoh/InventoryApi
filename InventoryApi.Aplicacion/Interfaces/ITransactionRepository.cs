using InventoryApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventoryApi.Aplicacion.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync(int? productoId = null, DateTime? fromDate = null, DateTime? toDate = null, string? tipo = null);

        // Ahora devuelve la transacción completa con ProductoNombre y StockActual poblados
        Task<Transaction> AddAsync(Transaction transaction);
    }
}
