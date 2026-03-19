using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace bmak_ecommerce.API.Controllers
{
    /// <summary>
    /// Proxy ảnh từ các domain bên ngoài để tránh lỗi CORS phía frontend.
    /// Chỉ cho phép proxy từ các domain đã được whitelist (chống SSRF).
    /// </summary>
    [Route("api/proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private static readonly HashSet<string> _allowedHosts = new(StringComparer.OrdinalIgnoreCase)
        {
            "ankhanhhouse.com",
            "www.ankhanhhouse.com"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(IHttpClientFactory httpClientFactory, ILogger<ProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private static bool IsLikelyImagePath(string path)
        {
            var ext = Path.GetExtension(path);
            return ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".png", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".webp", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".gif", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".avif", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase)
                   || ext.Equals(".svg", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// GET api/proxy/image?url=https://ankhanhhouse.com/wp-content/uploads/...
        /// Stream ảnh từ domain đã được whitelist về phía client.
        /// </summary>
        [HttpGet("image")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)] // cache 1 ngày
        public async Task<IActionResult> GetImage([FromQuery] string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Missing 'url' parameter.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
                || !_allowedHosts.Contains(uri.Host))
            {
                _logger.LogWarning("Proxy request blocked for URL: {Url}", url);
                return BadRequest("URL không hợp lệ hoặc domain không được phép.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BmakImageProxy", "1.0"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

                if (uri.Host.EndsWith("ankhanhhouse.com", StringComparison.OrdinalIgnoreCase))
                {
                    client.DefaultRequestHeaders.Referrer = new Uri($"{uri.Scheme}://{uri.Host}/");
                }

                var upstream = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (!upstream.IsSuccessStatusCode)
                {
                    upstream.Dispose();
                    return StatusCode((int)upstream.StatusCode, "Không thể tải ảnh từ nguồn.");
                }

                var contentType = upstream.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && !IsLikelyImagePath(uri.AbsolutePath))
                {
                    upstream.Dispose();
                    return BadRequest("URL không trỏ đến một ảnh hợp lệ.");
                }

                var stream = await upstream.Content.ReadAsStreamAsync(cancellationToken);

                HttpContext.Response.RegisterForDispose(upstream);
                HttpContext.Response.RegisterForDispose(stream);

                if (upstream.Content.Headers.ContentLength.HasValue)
                    Response.Headers.ContentLength = upstream.Content.Headers.ContentLength.Value;

                Response.Headers["Cache-Control"] = "public,max-age=86400";
                Response.Headers["X-Proxy-Source"] = uri.Host;

                var finalContentType = contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                    ? contentType
                    : "image/jpeg";

                return File(stream, finalContentType);
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, "Timeout khi tải ảnh.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Lỗi khi proxy ảnh từ {Url}", url);
                return StatusCode(502, "Không thể kết nối đến nguồn ảnh.");
            }
        }
    }
}
