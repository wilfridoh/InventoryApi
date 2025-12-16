using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;


namespace InventoryApi.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly ITransactionRepository _repo;
        public TransactionsController(ILogger<TransactionsController> logger, ITransactionRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? productoId = null, DateTime? fromDate = null, DateTime? toDate = null, string? tipo = null)
        {
            var items = await _repo.GetAllAsync(productoId, fromDate, toDate, tipo);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            if (transaction is null) return BadRequest();

            var result = await _repo.AddAsync(transaction);

            // Devuelve la transacción enriquecida (incluye ProductoNombre y StockActual)
            return CreatedAtAction(nameof(GetAll), new { id = result.TransaccionId }, result);
        }
    }
}
