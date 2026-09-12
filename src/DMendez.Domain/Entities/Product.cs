namespace DMendez.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public Guid CategoryId { get; private set; }
        public bool IsActive { get; private set; }
        public string ImageUrl { get; private set; }

        private Product() {} // EF Core

        public Product(string name, string description, decimal price, Guid categoryId, string imageUrl = null)
        {
            Id = Guid.NewGuid();
            UpdateDetails(name, description);
            ChangePrice(price);
            ChangeCategory(categoryId);
            ImageUrl = imageUrl;

            IsActive = true;
        }

        public void UpdateImage(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public void UpdateDetails(string name, string description)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("El nombre del producto es obligatorio.", nameof(name));
            }

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
        }

        public void ChangePrice(decimal price)
        {
            if(price <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor.", nameof(price));
            }

            Price = price;
        }

        public void ChangeCategory(Guid categoryId)
        {
            if(categoryId == Guid.Empty)
            {
                throw new ArgumentException("La categoria es obligatoria.", nameof(categoryId));
            }

            CategoryId = categoryId;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Desactive()
        {
            IsActive = false;
        }
    }
}