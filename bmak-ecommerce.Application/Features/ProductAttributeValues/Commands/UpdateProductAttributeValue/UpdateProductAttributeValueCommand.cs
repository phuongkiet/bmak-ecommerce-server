using System.Text.Json.Serialization;

namespace bmak_ecommerce.Application.Features.ProductAttributeValues.Commands.UpdateProductAttributeValue
{
    public class UpdateProductAttributeValueCommand
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;
        public string? ExtraData { get; set; }

        public int AttributeId { get; set; }
    }
}
