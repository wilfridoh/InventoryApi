using InventoryApi.Aplicacion.Interfaces;
using InventoryApi.Domain.Entities;
using InventoryApi.API.Responses;
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
            var products = await _repo.GetAllAsync();
            var resp = ResponseBuilder.Build(200, new { productos = products });
            return Ok(resp);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                return NotFound(ResponseBuilder.Build(404, new { productos = Array.Empty<Product>() }, new[] { "Producto no encontrado." }));

            return Ok(ResponseBuilder.Build(200, new { productos = new[] { product } }));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (product is null) return BadRequest(ResponseBuilder.Build(400, (object?)null, new[] { "El cuerpo de la petición es requerido y debe ser JSON válido." }));
            product.FechaCreacion = DateTime.UtcNow;
            await _repo.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.ProductoId }, ResponseBuilder.Build(201, new { productos = new[] { product } }));
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

                return BadRequest(ResponseBuilder.Build(400, new { rawBody = string.IsNullOrWhiteSpace(raw) ? null : raw }, new[] { "Cuerpo inválido o JSON malformado.", "Asegúrate de enviar Content-Type: application/json y un JSON válido." }));
            }

            if (id != product.ProductoId) return BadRequest(ResponseBuilder.Build(400, (object?)null, new[] { "El id de la ruta no coincide con el id del producto." }));

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound(ResponseBuilder.Build(404, (object?)null, new[] { "Producto no encontrado." }));

            // Actualiza campos permitidos
            existing.Nombre = product.Nombre;
            existing.Descripcion = product.Descripcion;
            existing.Categoria = product.Categoria;
            existing.ImagenUrl = product.ImagenUrl;
            existing.Precio = product.Precio;
            existing.Stock = product.Stock;
            existing.FechaModificacion = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return Ok(ResponseBuilder.Build(200, new { productos = new[] { existing } }));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound(ResponseBuilder.Build(404, (object?)null, new[] { "Producto no encontrado." }));

            await _repo.DeleteAsync(existing);
            return Ok(ResponseBuilder.Build(200, new { productos = Array.Empty<Product>() }, new[] { "Producto eliminado." }));
        }
    }
}
