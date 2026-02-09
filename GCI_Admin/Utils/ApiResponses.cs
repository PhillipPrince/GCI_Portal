using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    // Generic version for type-safe responses
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string Code { get; set; } = "200";
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    // Non-generic version for simple responses
    public class ApiResponse
    {
        public bool IsSuccess { get; set; } = true;
        public string Code { get; set; } = "200";
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
