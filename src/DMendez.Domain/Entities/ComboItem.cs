namespace DMendez.Domain.Entities
{
    public class ComboItem
    {
        public Guid Id { get; private set; }
        public Guid ComboId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private ComboItem() {} // EF Core

        public ComboItem(
            Guid comboId,
            Guid productId,
            int quantity)
        {
            if (comboId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El combo es obligatorio.",
                    nameof(comboId));
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

            Id = Guid.NewGuid();
            ComboId = comboId;
            ProductId = productId;
            Quantity = quantity;
        }

        public void ChangeQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "La cantidad debe ser mayor que cero.");
            }

            Quantity = quantity;
        }
    }
}