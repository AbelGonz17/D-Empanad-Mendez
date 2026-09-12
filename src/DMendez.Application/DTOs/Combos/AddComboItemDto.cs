using System;

namespace DMendez.Application.DTOs.Combos
{
    public class AddComboItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
