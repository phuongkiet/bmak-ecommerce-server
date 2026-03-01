using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.ProductAttributes.Commands.UpdateProductAttribute
{
    public class UpdateProductAttributeCommand
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Code { get; set; }
        public string? Description { get; set; }

    }
}
