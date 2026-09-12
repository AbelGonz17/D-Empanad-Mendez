namespace DMendez.Domain.Entities
{
    public class Combo
    {
        private readonly List<ComboItem> _items = new();

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public string ImageUrl { get; private set; }

        public IReadOnlyCollection<ComboItem> Items => _items.AsReadOnly();

        private Combo() {} // EF Core

        public Combo(string name, decimal price)
        {
            Id = Guid.NewGuid();
            Name = ValidateName(name);

            ChangePrice(price);

            // Se activa después de agregar sus componentes.
            IsActive = false;
        }

        public void Rename(string name)
        {
            Name = ValidateName(name);
        }

        public void UpdateImage(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public void ChangePrice(decimal price)
        {
            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(price),
                    "El precio debe ser mayor que cero.");
            }

            Price = price;
        }

        public void AddItem(Guid productId, int quantity)
        {
            if (_items.Any(item => item.ProductId == productId))
            {
                throw new InvalidOperationException(
                    "El producto ya pertenece al combo.");
            }

            var item = new ComboItem(Id, productId, quantity);

            _items.Add(item);
        }

        public void ChangeItemQuantity(Guid productId, int quantity)
        {
            var item = FindItem(productId);

            item.ChangeQuantity(quantity);
        }

        public void RemoveItem(Guid productId)
        {
            var item = FindItem(productId);

            if (IsActive && _items.Count == 1)
            {
                throw new InvalidOperationException(
                    "Un combo activo debe conservar al menos un componente.");
            }

            _items.Remove(item);
        }

        public void Activate()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException(
                    "Agrega al menos un componente antes de activar el combo.");
            }

            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private ComboItem FindItem(Guid productId)
        {
            return _items.FirstOrDefault(item => item.ProductId == productId)
                ?? throw new InvalidOperationException(
                    "El producto no pertenece al combo.");
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre del combo es obligatorio.",
                    nameof(name));
            }

            return name.Trim();
        }
    }
}