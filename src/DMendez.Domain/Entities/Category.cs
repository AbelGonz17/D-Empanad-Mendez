namespace DMendez.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public string ImageUrl { get; private set; }

        private Category() {} // EF Core

        public Category (string name, string imageUrl = null)
        {
            Id = Guid.NewGuid();
            Name = ValidateName(name);
            ImageUrl = imageUrl;
            IsActive = true;
        }

        public void UpdateImage(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public void Rename(string name)
        {
            Name = ValidateName(name);
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Desactive()
        {
            IsActive = false;  
        }

        private static string ValidateName (string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "El nombre de la categoria es obligatorio", 
                    nameof(name));
            }

            return name.Trim();
        }
    }
}