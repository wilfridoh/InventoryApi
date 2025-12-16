using System.Net;
using System.Text.Json;

namespace InventoryApi.API.Middlewares
{
    public class MiddlewareGlobal
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareGlobal> _logger;

        public MiddlewareGlobal(RequestDelegate next, ILogger<MiddlewareGlobal> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error no controlado");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Ocurrió un error en el servidor",
                    detail = ex.Message
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
