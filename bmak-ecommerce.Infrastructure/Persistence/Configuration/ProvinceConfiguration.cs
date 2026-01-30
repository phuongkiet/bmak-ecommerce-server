using bmak_ecommerce.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Domain.Entities.Directory;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            // 1. Tên bảng
            builder.ToTable("Provinces");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

        }
    }
}
