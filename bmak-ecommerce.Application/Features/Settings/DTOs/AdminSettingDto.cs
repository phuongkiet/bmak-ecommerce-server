namespace bmak_ecommerce.Application.Features.Settings.DTOs
{
    public class AdminSettingDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string Hotline { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
    }
}
