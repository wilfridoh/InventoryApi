using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApi.Domain.Entities
{
    public class Transaction
    {
        public long TransaccionId { get; set; }
        public string Tipo { get; set; } = null!;
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal => PrecioUnitario * Cantidad;
        public string? Detalle { get; set; }
        public DateTime Fecha { get; set; }

        // Campos devueltos por sp_ObtenerTransacciones -> no son columnas físicas de la tabla Transacciones
        [NotMapped]
        public string? ProductoNombre { get; set; }

        [NotMapped]
        public int? StockActual { get; set; }
    }
}
