using System;

namespace DMendez.Application.DTOs.Orders
{
    public class OrderItemDto
    {
        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
