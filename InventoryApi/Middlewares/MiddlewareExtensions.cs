namespace InventoryApi.API.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MiddlewareGlobal>();
        }
    }
}
