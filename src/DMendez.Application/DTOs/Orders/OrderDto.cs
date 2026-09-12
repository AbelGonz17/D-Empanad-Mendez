using System;
using System.Collections.Generic;
using DMendez.Domain.Enums;

namespace DMendez.Application.DTOs.Orders
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public OrderType Type { get; set; }
        public string TypeName => Type.ToString();
        public string? DeliveryAddress { get; set; }
        public Guid? DeliveryZoneId { get; set; }
        public decimal DeliveryFee { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodName => PaymentMethod.ToString();
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public IReadOnlyList<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
