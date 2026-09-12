using System;
using System.Collections.Generic;
using DMendez.Domain.Enums;

namespace DMendez.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string UserId { get; set; } = string.Empty;
        public OrderType Type { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? DeliveryAddress { get; set; }
        public Guid? DeliveryZoneId { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
