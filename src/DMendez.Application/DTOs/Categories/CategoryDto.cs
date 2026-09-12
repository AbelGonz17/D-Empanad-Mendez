using System;

namespace DMendez.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}
