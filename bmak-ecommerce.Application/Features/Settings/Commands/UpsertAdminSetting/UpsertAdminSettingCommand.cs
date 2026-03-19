namespace bmak_ecommerce.Application.Features.Settings.Commands.UpsertAdminSetting
{
    public class UpsertAdminSettingCommand
    {
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string Hotline { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
    }
}
