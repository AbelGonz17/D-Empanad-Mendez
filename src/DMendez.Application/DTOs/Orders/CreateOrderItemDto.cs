using System;

namespace DMendez.Application.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}
