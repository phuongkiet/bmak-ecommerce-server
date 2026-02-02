using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Models
{
    /// <summary>
    /// Wrapper chung cho tất cả API Response
    /// </summary>
    public class ApiResponse<T>
    {
        public T? Value { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(T? value, bool isSuccess = true, string? message = null)
        {
            Value = value;
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ApiResponse<T> Success(T? value, string? message = "Success")
        {
            return new ApiResponse<T>(value, true, message);
        }

        public static ApiResponse<T> Failure(string message)
        {
            return new ApiResponse<T>(default, false, message);
        }
    }

    /// <summary>
    /// Wrapper cho response không có data (VD: Update/Delete)
    /// </summary>
    public class ApiResponse
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(bool isSuccess = true, string? message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ApiResponse Success(string? message = "Success")
        {
            return new ApiResponse(true, message);
        }

        public static ApiResponse Failure(string message)
        {
            return new ApiResponse(false, message);
        }
    }
}
