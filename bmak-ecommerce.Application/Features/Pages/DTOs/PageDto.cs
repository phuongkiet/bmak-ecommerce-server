using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Application.Features.Pages.DTOs
{
    public class PageDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public List<PageSectionDto> Sections { get; set; }
    }

    public class PageSectionDto
    {
        public string Id { get; set; }
        public string Type { get; set; } // 'hero', 'product-carousel', v.v.
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePosition { get; set; }
        public List<HeroSlideDto> HeroSlides { get; set; }
        public ProductCarouselConfigDto CarouselConfig { get; set; }
    }

    public class HeroSlideDto
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Badge { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
    }

    public class ProductCarouselConfigDto
    {
        public string ListType { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public int? Limit { get; set; }
    }
}
