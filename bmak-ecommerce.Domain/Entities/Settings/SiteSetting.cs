using bmak_ecommerce.Domain.Common;

namespace bmak_ecommerce.Domain.Entities.Settings
{
    public class SiteSetting : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string Hotline { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
    }
}
