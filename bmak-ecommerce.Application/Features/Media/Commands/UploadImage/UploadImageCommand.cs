using bmak_ecommerce.Application.Features.Media.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.Commands.UploadImage
{
    public class UploadImageCommand
    {
        public IFormFile File { get; set; }
        public string? AltText { get; set; } // Optional: Nhập luôn lúc upload
    }
}
