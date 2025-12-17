using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using InventoryApi.API.Responses;
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
            return Ok(ResponseBuilder.Build(200, new { transacciones = items }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            if (transaction is null) return BadRequest(ResponseBuilder.Build(400, (object?)null, new[] { "El cuerpo de la petición es requerido y debe ser JSON válido." }));

            var result = await _repo.AddAsync(transaction);

            // Devuelve la transacción enriquecida (incluye ProductoNombre y StockActual)
            return CreatedAtAction(nameof(GetAll), new { id = result.TransaccionId }, ResponseBuilder.Build(201, new { transacciones = new[] { result } }));
        }
    }
}
