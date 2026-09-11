namespace DMendez.Domain.Entities
{
    public class ProductInventory
    {
        public Guid ProductId { get; private set; }
        public int StockQuantity { get; private set; }
        public int ReserveQuanity { get; private set; }
        public int AvaibleQuantity => StockQuantity - ReserveQuanity;

        public ProductInventory(Guid productId, int initialQuantity)
        {
            if (ProductId == Guid.Empty)
            {
                throw new ArgumentException("El producto es obligatorio.", nameof(ProductId));
            }

            if (initialQuantity < 0)
            {
                throw new ArgumentException(nameof(initialQuantity), "La cantidad inicial no puede ser negativa");
            }

            ProductId = productId;
            StockQuantity = initialQuantity;
        }

        public void AddStock(int quantity)
        {
            ValidatePositiveQuantity(quantity);

            if (quantity > AvaibleQuantity)
            {
                throw new InvalidOperationException("No hay suficiente unidades disponibles.");
            }

            ReserveQuanity += quantity;
        }

        public void ReleaseReservation(int quantity)
        {
            ValidateReservedQuantity(quantity);

            ReserveQuanity -= quantity;
        }

        public void ConsumeReservation(int quantity)
        {
            ValidateReservedQuantity(quantity);

            StockQuantity -= quantity;
            ReserveQuanity -= quantity;
        }

        private void ValidateReservedQuantity(int quantity)
        {
            ValidatePositiveQuantity(quantity);

            if (quantity > ReserveQuanity)
            {
                throw new InvalidOperationException("La cantidad supera las unidades reservadas.");
            }
        }

        private static void ValidatePositiveQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero");
            }
        }
    }
}