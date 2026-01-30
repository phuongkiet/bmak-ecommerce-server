using bmak_ecommerce.Domain.Entities.Directory;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Infrastructure.Persistence.Configuration
{
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            // 1. Tên bảng
            builder.ToTable("Wards");

            // 2. Khóa chính
            builder.HasKey(x => x.Id);

            builder.HasOne(w => w.Province)       // 1. Ward có 1 Province
                   .WithMany(p => p.Wards)        // 2. Province có nhiều Wards (List<Ward> ở trên)
                   .HasForeignKey(w => w.ProvinceId) // 3. Khóa ngoại là Ward.ProvinceId
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
