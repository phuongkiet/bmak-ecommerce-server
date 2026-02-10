using AutoMapper;
using bmak_ecommerce.Application.Common.Interfaces;
using bmak_ecommerce.Domain.Models;
using bmak_ecommerce.Application.Features.Products.DTOs.Catalog;
using bmak_ecommerce.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bmak_ecommerce.Application.Common.Models;
using bmak_ecommerce.Application.Common.Attributes;

namespace bmak_ecommerce.Application.Features.Products.Queries.Products.GetProductById
{
    [AutoRegister]

    public class GetProductByIdHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductDto?>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Gọi hàm Repository chuyên dụng đã viết ở Bước 1
            var product = await _unitOfWork.Products.GetProductDetailAsync(request.Id);

            // 2. Kiểm tra Null -> Trả về Failure
            if (product == null)
            {
                return Result<ProductDto?>.Failure($"Không tìm thấy sản phẩm có ID = {request.Id}");
            }

            // 3. Map Entity -> DTO
            var productDto = _mapper.Map<ProductDto>(product);

            // 4. (Tùy chọn) Logic bổ sung nếu cần
            // Ví dụ: Tính tổng tồn kho từ list Stocks
            if (product.Stocks != null)
            {
                productDto.StockQuantity = product.Stocks.Sum(x => x.QuantityOnHand);
            }

            // 5. Trả về Success
            return Result<ProductDto?>.Success(productDto);
        }
    }
}
