using bmak_ecommerce.Domain.Common;

namespace bmak_ecommerce.Domain.Entities.Catalog
{
    public class ProductAttributeSelection : BaseEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int AttributeId { get; set; }
        public virtual ProductAttribute Attribute { get; set; }

        public int AttributeValueId { get; set; }
        public virtual ProductAttributeValue AttributeValue { get; set; }
    }
}
