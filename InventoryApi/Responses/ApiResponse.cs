using System;
using System.Collections.Generic;

namespace InventoryApi.API.Responses
{
    public class ApiResponse<T>
    {
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public int StatusCode { get; set; }
        public IEnumerable<string> Messages { get; set; } = Array.Empty<string>();
        public T Data { get; set; } = default!;
    }

    public static class ResponseBuilder
    {
        public static ApiResponse<T> Build<T>(int statusCode, T data, IEnumerable<string>? messages = null)
        {
            return new ApiResponse<T>
            {
                Token = Guid.NewGuid().ToString(),
                StatusCode = statusCode,
                Messages = messages ?? Array.Empty<string>(),
                Data = data
            };
        }
    }
}
