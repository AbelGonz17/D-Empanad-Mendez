namespace DMendez.Application.DTOs.Combos
{
    public class UpdateComboDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
