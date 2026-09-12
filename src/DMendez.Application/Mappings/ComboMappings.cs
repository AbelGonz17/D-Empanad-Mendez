using System.Collections.Generic;
using System.Linq;
using DMendez.Application.DTOs.Combos;
using DMendez.Domain.Entities;

namespace DMendez.Application.Mappings
{
    public static class ComboMappings
    {
        public static ComboDto ToDto(this Combo combo)
        {
            return new ComboDto
            {
                Id = combo.Id,
                Name = combo.Name,
                Price = combo.Price,
                IsActive = combo.IsActive,
                ImageUrl = combo.ImageUrl,
                Items = combo.Items.Select(i => i.ToDto()).ToList()
            };
        }

        public static ComboItemDto ToDto(this ComboItem item)
        {
            return new ComboItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
        }

        public static IReadOnlyList<ComboDto> ToDtoList(this IEnumerable<Combo> combos)
        {
            return combos.Select(c => c.ToDto()).ToList();
        }
    }
}
