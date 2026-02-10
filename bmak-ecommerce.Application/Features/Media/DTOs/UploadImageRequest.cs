using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Media.DTOs
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; }
    }
}
