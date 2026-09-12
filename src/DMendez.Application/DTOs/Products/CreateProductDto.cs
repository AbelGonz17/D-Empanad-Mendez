using System;

namespace DMendez.Application.DTOs.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public int InitialStock { get; set; }
    }
}
