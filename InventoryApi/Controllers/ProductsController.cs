using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductRepository _repo;

        public ProductsController(ILogger<ProductsController> logger, IProductRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repo.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (product is null) return BadRequest("El cuerpo de la petición es requerido y debe ser JSON válido.");
            product.FechaCreacion = DateTime.UtcNow;
            await _repo.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductoId }, product);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(int id, [FromBody] Product? product)
        {
            // Si el binding falló (producto nulo) intentamos devolver información útil al cliente
            if (product is null)
            {
                // Permitir lectura del body para devolver raw JSON (si existe)
                Request.EnableBuffering();
                Request.Body.Position = 0;
                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                var raw = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                return BadRequest(new
                {
                    title = "Cuerpo inválido o JSON malformado.",
                    detail = "Asegúrate de enviar Content-Type: application/json y un JSON válido.",
                    rawBody = string.IsNullOrWhiteSpace(raw) ? null : raw
                });
            }

            if (id != product.ProductoId) return BadRequest("El id de la ruta no coincide con el id del producto.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            // Actualiza campos permitidos
            existing.Nombre = product.Nombre;
            existing.Descripcion = product.Descripcion;
            existing.Categoria = product.Categoria;
            existing.ImagenUrl = product.ImagenUrl;
            existing.Precio = product.Precio;
            existing.Stock = product.Stock;
            existing.FechaModificacion = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }
    }
}
