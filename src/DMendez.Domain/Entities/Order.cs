using System;
using System.Collections.Generic;
using System.Linq;
using DMendez.Domain.Enums;

namespace DMendez.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public DateTimeOffset OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public OrderType Type { get; private set; }
        public string DeliveryAddress { get; private set; }
        public Guid? DeliveryZoneId { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal SubTotal => _items.Sum(i => i.TotalPrice);
        public decimal TotalAmount => SubTotal + DeliveryFee;

        private Order() {} // EF Core

        public Order(
            string userId, 
            OrderType type, 
            PaymentMethod paymentMethod, 
            string deliveryAddress = null, 
            Guid? deliveryZoneId = null,
            decimal deliveryFee = 0)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("El cliente es obligatorio.", nameof(userId));

            if (type == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("La dirección de entrega es obligatoria para pedidos a domicilio.");

            Id = Guid.NewGuid();
            UserId = userId;
            OrderDate = DateTimeOffset.UtcNow;
            Status = OrderStatus.Pending;
            Type = type;
            PaymentMethod = paymentMethod;
            DeliveryAddress = deliveryAddress;
            DeliveryZoneId = deliveryZoneId;
            DeliveryFee = type == OrderType.Delivery ? deliveryFee : 0;
        }

        public void AddItem(Guid? productId, Guid? comboId, int quantity, decimal unitPrice)
        {
            var item = new OrderItem(Id, productId, comboId, quantity, unitPrice);
            _items.Add(item);
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            // Podría haber validaciones de máquina de estados aquí (ej: de Pending a Preparing, etc.)
            Status = newStatus;
        }
    }
}
