using System.Collections.Generic;
using System.Linq;
using DMendez.Application.DTOs.Products;
using DMendez.Domain.Entities;

namespace DMendez.Application.Mappings
{
    public static class ProductMappings
    {
        public static ProductDto ToDto(this Product product, ProductInventory? inventory = null)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                StockQuantity = inventory?.StockQuantity ?? 0,
                AvailableQuantity = inventory?.AvaibleQuantity ?? 0
            };
        }

        public static IReadOnlyList<ProductDto> ToDtoList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToDto()).ToList();
        }

        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product(dto.Name, dto.Description, dto.Price, dto.CategoryId, dto.ImageUrl);
        }
    }
}
