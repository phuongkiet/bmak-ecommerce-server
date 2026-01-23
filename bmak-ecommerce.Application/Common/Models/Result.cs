using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }

        // Constructor private để bắt buộc dùng hàm static Success/Failure
        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        // Tạo kết quả thành công
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        // Tạo kết quả thất bại
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }
    }

    // Class dùng cho các hàm không trả về dữ liệu (VD: Update/Delete)
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
        {
            return new Result(true, null);
        }

        public static Result Failure(string error)
        {
            return new Result(false, error);
        }
    }
}
