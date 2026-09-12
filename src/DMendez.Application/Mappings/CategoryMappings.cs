using System.Collections.Generic;
using System.Linq;
using DMendez.Application.DTOs.Categories;
using DMendez.Domain.Entities;

namespace DMendez.Application.Mappings
{
    public static class CategoryMappings
    {
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                ImageUrl = category.ImageUrl
            };
        }

        public static IReadOnlyList<CategoryDto> ToDtoList(this IEnumerable<Category> categories)
        {
            return categories.Select(c => c.ToDto()).ToList();
        }

        public static Category ToEntity(this CreateCategoryDto dto)
        {
            return new Category(dto.Name, dto.ImageUrl);
        }
    }
}
