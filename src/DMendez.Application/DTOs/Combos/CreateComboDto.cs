using System.Collections.Generic;

namespace DMendez.Application.DTOs.Combos
{
    public class CreateComboDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<ComboItemDto> Items { get; set; } = new();
    }
}
