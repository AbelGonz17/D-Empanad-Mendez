using System;

namespace DMendez.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid? ProductId { get; private set; }
        public Guid? ComboId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        private OrderItem() {} // EF Core

        public OrderItem(Guid orderId, Guid? productId, Guid? comboId, int quantity, decimal unitPrice)
        {
            if (productId == null && comboId == null)
                throw new ArgumentException("Debe especificar un producto o un combo.");

            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(unitPrice));

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            ComboId = comboId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
