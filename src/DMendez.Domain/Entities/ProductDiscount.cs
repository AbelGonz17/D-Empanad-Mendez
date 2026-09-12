namespace DMendez.Domain.Entities
{
    public class ProductDiscount
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal Percentage { get; private set; }
        public DateTimeOffset StartsAt { get; private set; }
        public DateTimeOffset EndsAt { get; private set; }
        public bool IsActive { get; private set; }

        private ProductDiscount() {} // EF Core

        public ProductDiscount(
            Guid productId,
            decimal percentage,
            DateTimeOffset startsAt,
            DateTimeOffset endsAt)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El producto es obligatorio.",
                    nameof(productId));
            }

            if (percentage <= 0 || percentage > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(percentage),
                    "El porcentaje debe ser mayor que cero y hasta 100.");
            }

            if (endsAt <= startsAt)
            {
                throw new ArgumentException(
                    "El fin debe ser posterior al inicio.",
                    nameof(endsAt));
            }

            Id = Guid.NewGuid();
            ProductId = productId;
            Percentage = percentage;
            StartsAt = startsAt;
            EndsAt = endsAt;
            IsActive = true;
        }

        public bool IsApplicableAt(DateTimeOffset now)
        {
            return IsActive
                && now >= StartsAt
                && now < EndsAt;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}