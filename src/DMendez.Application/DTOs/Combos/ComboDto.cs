using System;
using System.Collections.Generic;

namespace DMendez.Application.DTOs.Combos
{
    public class ComboDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public IReadOnlyList<ComboItemDto> Items { get; set; } = new List<ComboItemDto>();
    }
}
