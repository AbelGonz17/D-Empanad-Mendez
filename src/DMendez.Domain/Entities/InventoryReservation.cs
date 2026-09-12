using DMendez.Domain.Enums;

namespace DMendez.Domain.Entities
{
    public class InventoryReservation
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }

        public InventoryReservationStatus Status { get; private set; }

        private InventoryReservation() {} // EF Core

        public InventoryReservation(
            Guid orderId,
            Guid productId,
            int quantity,
            DateTimeOffset createdAt,
            DateTimeOffset? expiresAt)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El pedido es obligatorio.",
                    nameof(orderId));
            }

            if (productId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El producto es obligatorio.",
                    nameof(productId));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad debe ser mayor que cero.");
            }

            if (expiresAt.HasValue && expiresAt.Value <= createdAt)
            {
                throw new ArgumentException(
                    "El vencimiento debe ser posterior a la creación.",
                    nameof(expiresAt));
            }

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            Status = InventoryReservationStatus.Active;
        }

        public bool HasReachedExpiration(DateTimeOffset now)
        {
            return Status == InventoryReservationStatus.Active
                && ExpiresAt.HasValue
                && now >= ExpiresAt.Value;
        }

        public void Consume()
        {
            EnsureActive();

            Status = InventoryReservationStatus.Consumed;
        }

        public void Release()
        {
            EnsureActive();

            Status = InventoryReservationStatus.Released;
        }

        public void RemoveExpiration()
        {
            EnsureActive();

            ExpiresAt = null;
        }

        private void EnsureActive()
        {
            if (Status != InventoryReservationStatus.Active)
            {
                throw new InvalidOperationException(
                    "La reserva ya fue consumida o liberada.");
            }
        }
    }
}