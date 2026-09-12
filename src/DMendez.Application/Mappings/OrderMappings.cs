using System.Collections.Generic;
using System.Linq;
using DMendez.Application.DTOs.Orders;
using DMendez.Domain.Entities;

namespace DMendez.Application.Mappings
{
    public static class OrderMappings
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Type = order.Type,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryZoneId = order.DeliveryZoneId,
                DeliveryFee = order.DeliveryFee,
                PaymentMethod = order.PaymentMethod,
                SubTotal = order.SubTotal,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => i.ToDto()).ToList()
            };
        }

        public static OrderItemDto ToDto(this OrderItem item)
        {
            return new OrderItemDto
            {
                ProductId = item.ProductId,
                ComboId = item.ComboId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            };
        }

        public static IReadOnlyList<OrderDto> ToDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}
