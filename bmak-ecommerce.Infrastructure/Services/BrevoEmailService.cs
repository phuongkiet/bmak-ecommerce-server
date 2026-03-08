using bmak_ecommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Task = System.Threading.Tasks.Task;

namespace bmak_ecommerce.Infrastructure.Services
{
    public class BrevoEmailService : IEmailService
    {
        private readonly ILogger<BrevoEmailService> _logger;
        private readonly TransactionalEmailsApi _apiInstance;

        public BrevoEmailService(IConfiguration configuration, ILogger<BrevoEmailService> logger)
        {
            _logger = logger;

            // 1. Lấy API Key từ appsettings.json
            string apiKey = configuration["BrevoSettings:ApiKey"]
                            ?? throw new ArgumentNullException("Không tìm thấy BrevoSettings:ApiKey trong appsettings.json");

            // 2. Cấu hình SDK của Brevo
            // Xóa key cũ đi (nếu có) để tránh lỗi khi DI khởi tạo lại Service
            if (Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                Configuration.Default.ApiKey.Remove("api-key");
            }
            Configuration.Default.ApiKey.Add("api-key", apiKey);

            // 3. Khởi tạo Api Instance để gửi mail
            _apiInstance = new TransactionalEmailsApi();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                // Cấu hình người gửi
                var sender = new SendSmtpEmailSender(name: "An Khanh Store", email: "kietnp.ankhanh@gmail.com");

                // Cấu hình người nhận
                var to = new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo(email: toEmail)
                };

                // Đóng gói payload
                var sendSmtpEmail = new SendSmtpEmail(
                    sender: sender,
                    to: to,
                    subject: subject,
                    htmlContent: htmlContent
                );

                // Gọi API gửi mail
                await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);

                _logger.LogInformation($"[Brevo] Đã gửi email tới {toEmail} thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Brevo] Lỗi khi gửi email tới {toEmail}. API có thể đang lỗi hoặc sai Key.");
                // Ném lỗi ra ngoài. Điều này RẤT QUAN TRỌNG để Consumer của RabbitMQ 
                // biết là gửi lỗi, từ đó nó mới nhét message lại vào Queue để thử lại sau (Retry).
                throw;
            }
        }
    }
}